using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class CharacterPlayerController : MonoBehaviour
{
    //Enum Movement Player Status 
    public enum PLAYER_STATUS
    {
        GROUND,
        JUMP,
        CROUCH,
        AIR,
        WALL,
        DASH,
        HANGED,
        NONE
    }

    public enum PLAYER_FACE_DIRECTION
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        RIGHT_UP,
        RIGHT_DOWN,
        LEFT_UP,
        LEFT_DOWN
    }

    public enum UNLOCK_HABILITIES
    {
        AIR_DASH,
        DOUBLE_JUMP,
        NONE
    }

    [Header("Player Movement Status")]
    [Space(5)]

    [TextArea] public string playerStateMessage;

    [Header("Unlock Habilities")]
    [Space(5)]

    public bool unlockAirDash;
    public bool unlockDoubleJump;

    [Header("Physics Variables")]

    //Physics Variables
    [Header("__________________________ SPEED __________________________")]
    public float speed;
    public float maxSpeedX;
    public float maxSpeedY;
    public float airSpeedReduction;
    public float icedFloorEffect;
    [Header("__________________________ CROUCH __________________________")]
    public float crouchSpeedReduction;
    public float multiplierAirCrouch;
    [Header("__________________________ JUMP __________________________")]
    public float minJumpForce;
    public float maxJumpTime;
    public float jumpForceMultiplier;
    public float fallmultiplier;
    public float coyoteTime;
    [Header("__________________________ DASH __________________________")]
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    [Header("_________________________ EARRING _________________________")]
    public float maxAngleFloor;
    public float minAngleSlide;
    [Header("_________________________ HANG WALLS _________________________")]
    public float hangWallImpulseUp;
    public float hangWallImpulseSides;
    [Header("_________________________ SLIDING _________________________")]
    public float slidingForce;
    [Header("_________________________ INPUT BUFFER _________________________")]
    public float maxTimeInputBuffer;

    private float spaceTime;
    private float dashTime;
    private float cooldownDashTime;
    private float playerDir;
    private float earringFloor;
    private float coyoteTimeCounter;
    private float gravityEffect;

    private int maxAirJumps;

    [Header("_________________________ COMBAT _________________________")]
    //Hit variables
    public float impactHit;

    [Header("________________________ ANIMATOR ________________________")]
    //Animator
    private Animator animator;

    private SpriteRenderer playerSprite;

    //[Header("Movement Direction Input")]

    private Vector2 move;

    //Rigid Body
    [SerializeField] public Rigidbody2D rb;

    [Header("Checkers of Movement")]
    [Space(5)]

    //Checkers
    public bool isGrounded;
    private bool isRoof;
    private bool isSlide;
    private bool isCrouch;
    private bool isLeftWall;
    private bool isRightWall;
    private bool isHangingWall;
    private bool isUnderground;
    public bool isHangingEdge;
    public bool flipAnimation;
    public bool isTooMuchEarring;
    public bool doubleJump;
    public bool canDash;
    public bool canUnhang;
    public bool playerOnEdgeUnfrezze;

    public bool hasImpactHit;
    [SerializeField]public bool isImpactHitting;
    [SerializeField]public bool isDashing;

    public bool canJump;

    [SerializeField] public bool jumpStopper;
    bool moveStopper;
    bool cheatMode;

    [Space(10)]

    [Header("Collider Detectors")]
    [Space(5)]

    //Detection Colliders
    public BoxCollider2D GroundCheck;
    public BoxCollider2D LeftWallCheck;
    public BoxCollider2D RightWallCheck;
    public BoxCollider2D RoofDetector;
    [Space(10)]

    [Header("Crouch Colliders")]
    [Space(5)]

    public BoxCollider2D playerUpCollider;
    public BoxCollider2D UpCrouchCollider;
    public CircleCollider2D DownCrouchCollider;
    [Space(10)]

    [Header("Player Sprites")]
    [Space(5)]

    public Sprite Player_Full;
    public Sprite Player_Up;
    public Sprite Player_Down;
    [Space(10)]

    [Header("Assigned Layers")]
    [Space(5)]

    //Layers
    public LayerMask groundMask;
    public LayerMask slideMask;
    public LayerMask wallMask;
    public LayerMask roofMask;
    [Space(10)]

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference movement;
    public InputActionReference jumpingHold;
    public InputActionReference jumpingDown;
    public InputActionReference crouchingHold;
    public InputActionReference crouchingDown;
    public InputActionReference dashing;
    public InputActionReference impactHittingHold;
    public InputActionReference impactHittingDown;
    public InputActionReference UpAction;
    public InputActionReference DownAction;
    public InputActionReference LeftAction;
    public InputActionReference RightAction;
    public InputActionReference DropAction;
    public InputActionReference ClimbEdgesAction;
    [Space(10)]

    //Bool keys
    [SerializeField]public bool jumpKeyRelease;
    [SerializeField] public bool jumpKeyHold;
    [SerializeField] public bool climbEdges;
    bool impactHitHold;
    bool impactHitDown;
    bool dashDown;
    bool crouchDown;
    bool crouchHold;
    bool upKey;
    bool downKey;
    bool leftKey;
    bool rightKey;
    bool dropDown;

    [Header("Player Material")]
    [Space(5)]

    public Material player_Material;
    [Space(10)]

    //Actual Player state on Movement
    [SerializeField]public PLAYER_STATUS playerState;

    PLAYER_FACE_DIRECTION playerFaceDir;
    PLAYER_FACE_DIRECTION dashFacing;
    UNLOCK_HABILITIES habilityUnlocker;

    private Vector2 gravityVector;

    //Scripts
    private PlayerCombat combatScript;

    //Input Buffer
    Dictionary<PLAYER_STATUS, float> inputBufferSaver;

    private void OnEnable()
    {
        jumpingHold.action.started += JumpingHoldEvent;
        jumpingDown.action.started += JumpingDownEvent;
        impactHittingHold.action.started += ImpactHitHoldEvent;
        impactHittingDown.action.started += ImpactHitDownEvent;
        dashing.action.started += DashDownEvent;
        crouchingDown.action.started += CrouchDownEvent;
        crouchingHold.action.started += CrouchHoldEvent;
        UpAction.action.started += UpEvent;
        DownAction.action.started += DownEvent;
        LeftAction.action.started += LeftEvent;
        RightAction.action.started += RightEvent;
        DropAction.action.started += DropEvent;
        ClimbEdgesAction.action.started += ClimbEdgesEvent;
    }

    private void OnDisable()
    {
        jumpingHold.action.started -= JumpingHoldEvent;
        jumpingDown.action.started -= JumpingDownEvent;
        impactHittingHold.action.started -= ImpactHitHoldEvent;
        impactHittingDown.action.started -= ImpactHitDownEvent;
        dashing.action.started -= DashDownEvent;
        crouchingDown.action.started -= CrouchDownEvent;
        crouchingHold.action.started -= CrouchHoldEvent;
        UpAction.action.started -= UpEvent;
        DownAction.action.started -= DownEvent;
        LeftAction.action.started -= LeftEvent;
        RightAction.action.started -= RightEvent;
        DropAction.action.started -= DropEvent;
        ClimbEdgesAction.action.started -= ClimbEdgesEvent;
    }

    public void JumpingHoldEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpKeyHold = true;

            if (inputBufferSaver.ContainsKey(PLAYER_STATUS.JUMP))
            {
                inputBufferSaver[PLAYER_STATUS.JUMP] = maxTimeInputBuffer;
            }
            else
            {
                inputBufferSaver.Add(PLAYER_STATUS.JUMP, maxTimeInputBuffer);
            }
        }
        else
        {
            //jumpKeyHold = false;
        }

        if (context.canceled)
        {
            if ((isHangingWall || isGrounded || isHangingEdge) && !isCrouch)
            {
                jumpStopper = false;
            }

            if (!isImpactHitting && !hasImpactHit) //Return ImpactHit
            {
                hasImpactHit = true;
                isImpactHitting = false;
            }

            if (playerState == PLAYER_STATUS.JUMP)
            {
                JumpReset();
            }

            //HangWalls
            if (isHangingEdge)
            {
                canUnhang = true;
            }
        }
    }

    public void ImpactHitHoldEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            impactHitHold = true;
        }
        else
        {
            impactHitHold = false;
        }
    }

    public void JumpingDownEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpKeyRelease = true;
        }
        else
        {
            jumpKeyRelease = false;
        }
    }

    public void ImpactHitDownEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            impactHitDown = true;
        }
        else
        {
            impactHitDown = false;
        }
    }

    public void DashDownEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashDown = true;
        }
        else
        {
            dashDown = false;
        }
    }

    public void CrouchDownEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouchDown = true;
        }
        else
        {
            crouchDown = false;
        }
    } 
    
    public void CrouchHoldEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouchHold = true;
        }
        else
        {
            crouchHold = false;
        }
    }

    public void UpEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            upKey = true;
        }
        else
        {
            upKey = false;
        }
    }

    public void DownEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            downKey = true;
        }
        else
        {
            downKey = false;
        }
    }

    public void LeftEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftKey = true;
        }
        else
        {
            leftKey = false;
        }
    }

    public void RightEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightKey = true;
        }
        else
        {
            rightKey = false;
        }
    }

    public void DropEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dropDown = true;
        }
        else
        {
            dropDown = false;
        }
    }

    public void ClimbEdgesEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            climbEdges = true;
        }
        else
        {
            climbEdges = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gravityVector = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        combatScript = GetComponent<PlayerCombat>();
        inputBufferSaver = new Dictionary<PLAYER_STATUS, float>();

        gravityEffect = rb.gravityScale;

        maxAirJumps = 0;
        //maxSpeedX = 8; //Set the max X speed
        //maxSpeedY = 5; //Set the max Y speed

        hasImpactHit = false;
        isImpactHitting = false;
        jumpStopper = false;
        isCrouch = false;
        isUnderground = false;
        isDashing = false;
        doubleJump = false;
        flipAnimation = false;
        canUnhang = false;
        playerOnEdgeUnfrezze = false;

        playerFaceDir = PLAYER_FACE_DIRECTION.RIGHT; //To init the action, que put the player facing Right
        dashFacing = PLAYER_FACE_DIRECTION.RIGHT; //To init the action, que put the player dash facing Right
        habilityUnlocker = UNLOCK_HABILITIES.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //_JUMP
        Jumping();
        //JUMP_

        //_GROUND_HIT
        GroundHit();
        //GROUND_HIT_

        //_CHECK_GROUND_AND_SLIDE
        //Ground checker and Slide Mechanic
        CheckGroundAndSlide();
        //CHECK_GROUND_AND_SLIDE_

        //_CHECK_WALLS
        //Hanging Wall mechanic
        CheckWalls();
        //CHECK_WALLS_

        //HangingEdges
        if (isHangingEdge)
        {
            HangingEdges();
        }
    }

    private void Update()
    {
        //Input buffer update
        InputBufferUpdate();

        //Add the basic movement force to the player
        AddMovementSpeed();

        //_MOVEMENT
        //move = new Vector2(Input.GetAxisRaw("Horizontal"), 0); //Only exist horizontal control for basic movement 
        move.x = movement.action.ReadValue<Vector2>().x;
        //MOVEMENT_

        //_COYOTE_TIME
        CoyoteTime();
        //COYOTE_TIME_

        //_DASH
        Dash();
        //DASH_

        //_CROUCH
        CrouchingGroundAndAir();
        //CROUCH_

        //Check angle between player and ground in order to be able to Jump
        CheckEarringFloor();

        //Check Look Direction
        CheckPlayerFaceDirection();

        //Here we add an extra false gravity when falling
        FallDownGravity();

        //Cheater hability Unlocker, send a hability to this function to unlock it
        UnlockHabilities(habilityUnlocker);

        //Aniamte the player
        AnimatePlayer();

        if (dropDown && (playerState == PLAYER_STATUS.WALL || playerState == PLAYER_STATUS.HANGED)) //Player can deatach walls if press Left Control
        {
            PlayerUnFrezze();

            if (isHangingEdge)
            {
                canUnhang = false;
                playerOnEdgeUnfrezze = false;
                DownCrouchCollider.isTrigger = false;
                isHangingEdge = false;
            }

            playerState = PLAYER_STATUS.AIR;
        }

        //Rotate Player depending earring floor
        if (playerState == PLAYER_STATUS.GROUND || isSlide) //Check if player is on GROUND or Sliding to till it or not
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, earringFloor); //Tilts player
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 1); //Return player to Original rotation
        }

        //Deactivates emergency jumping stop, if is on ground, or if it was activated in hanging
        //if ((jumpKeyUp && isHangingWall || isGrounded) && !isCrouch)
        //{
        //    jumpStopper = false;
        //}

        //Update player status for the inspector
        playerStateMessage = playerState.ToString() + ", " + playerFaceDir.ToString();

        if (Input.GetKeyDown(KeyCode.Period)) //Alternate cheat mode
        {
            cheatMode = !cheatMode;
            Debug.Log("Cheats: " + cheatMode);
        }
    }

    //Here the inputs are check for turn off if they are active
    private void InputBufferUpdate()
    {
        for (int i = 0; i < (int)PLAYER_STATUS.NONE; i++) //Pass for all PLAYER_STATUS
        {
            if (inputBufferSaver.ContainsKey((PLAYER_STATUS)i))
            {
                if (inputBufferSaver[(PLAYER_STATUS)i] > 0) //Check if the timer is more than 0
                {
                    inputBufferSaver[(PLAYER_STATUS)i] -= Time.deltaTime; //Rest to the timer each time
                }
                else //Inc case timer reaches 0
                {
                    switch ((PLAYER_STATUS)i) //Serch for the input to turn off
                    {
                        case PLAYER_STATUS.JUMP:
                            if (jumpKeyHold)
                            {
                                jumpKeyHold = false;
                            }
                            break;
                        default: 
                            break;
                    }
                }
            }
        }
    }

    private void HangingEdges()
    {
        //if (canUnhang && playerState == PLAYER_STATUS.JUMP) //Exit from the hang edge situation with an impulse.
        //{
        //    PlayerUnFrezze();

        //    rb.AddForce(Vector2.up * 3);

        //    canUnhang = false;
        //    isHangingEdge = false;
        //}

        if (playerOnEdgeUnfrezze)
        {
            PlayerUnFrezze();
            JumpReset();
            canJump = true;

            playerState = PLAYER_STATUS.GROUND;

            canUnhang = false;
            isHangingEdge = false;
            DownCrouchCollider.isTrigger = false;
            playerOnEdgeUnfrezze = false;
        }

        if (playerState == PLAYER_STATUS.HANGED /*&& !jumpKeyHold*/) //Gain access to exit from the hang situation
        {
            canUnhang = true;
        }

        if (playerState != PLAYER_STATUS.HANGED && isHangingEdge) //Enter in the hang edge situation, frezzing the player, and giving the posibility to jump, also quit the actual jump of the player if it is happening.
        {
            PlayerFrezze();
            rb.linearVelocity = Vector2.zero;
            DownCrouchCollider.isTrigger = true;
            //canJump = true;
            canJump = false;

            if (jumpKeyHold && !jumpStopper)
            {
                jumpStopper = true;
            }

            playerState = PLAYER_STATUS.HANGED;
        }
    }

    //Cheater hability Unlocker
    private void UnlockHabilities(UNLOCK_HABILITIES unlocker)
    {
        if (cheatMode) //If is active with the numbers you can activate/deactivate habilities
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                unlocker = UNLOCK_HABILITIES.AIR_DASH;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                unlocker = UNLOCK_HABILITIES.DOUBLE_JUMP;
            }
        }

        //Here is the hability unlocker
        switch (unlocker)
        {
            case UNLOCK_HABILITIES.AIR_DASH:
                unlockAirDash = !unlockAirDash;
                Debug.Log("Air Dash: " + unlockAirDash);
                break;
            case UNLOCK_HABILITIES.DOUBLE_JUMP:
                unlockDoubleJump = !unlockDoubleJump;
                Debug.Log("Double Jump: " + unlockDoubleJump);
                break;
            default:
                unlocker = UNLOCK_HABILITIES.NONE;
                break;
        }

        unlocker = UNLOCK_HABILITIES.NONE;
    }

    //Here we add an extra false gravity when falling
    private void FallDownGravity()
    {
        if(playerState == PLAYER_STATUS.AIR && rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity -= new Vector2(0, gravityVector.y * fallmultiplier * Time.deltaTime);
        }
    }

    //Make the action of jumping
    private void Jumping()
    {
        if ((playerState == PLAYER_STATUS.AIR || playerState == PLAYER_STATUS.JUMP) && doubleJump && unlockDoubleJump) //Double jump ablity
        {
            if (jumpKeyHold && !impactHitHold && !isImpactHitting)
            {
                canJump = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
        }

        if (jumpKeyHold && !jumpStopper && !isTooMuchEarring && !isRoof && !isHangingEdge) //jumpStopper is an emergency stop jumping and take in consideration the earring of the floor in order to be able to jump, also check if there is no roof up the player or the earring of the floor is able to jump
        {
            //Check if you can Jump
            if (canJump)
            {
                maxAirJumps++;
                playerState = PLAYER_STATUS.JUMP;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(new Vector2(0, minJumpForce));

                if (unlockDoubleJump)
                {
                    doubleJump = !doubleJump;
                }

                canJump = false;
            }

            //Calculate the space pressing force in time
            if (playerState == PLAYER_STATUS.JUMP)
            {
                spaceTime += Time.deltaTime; //See how much time you press Space

                float smoothingTimer =  spaceTime / maxJumpTime; //This variable will produce the effect, were when you are at the half of the jump, will reduce the force added to the jump, making it smoother
                float finalJumpForce = 0; //This will be the force added to the jump

                if (smoothingTimer > 0.5f) //In case the jumps is more than the half of it, make it smoother
                {
                    finalJumpForce = jumpForceMultiplier * (1 - smoothingTimer); //We make the jump smoother, seeing the point of the jump that we are with smoothingTimer, reducing it by 1, and then we multiply it with "jumpForceMultiplier" to reducit
                }
                else
                {
                    finalJumpForce = jumpForceMultiplier; //In case we are not more than the half of the jump, we put the base force of jumping
                }

                rb.linearVelocity += new Vector2(0, gravityVector.y * finalJumpForce * Time.deltaTime); //Here finally add the force to the rigidbody, taking count the gravity force and the time.
            }
        }

        //Reset Jump if the player key up Space, or if arrive to maxJumpTime
        if (playerState == PLAYER_STATUS.JUMP && (spaceTime >= maxJumpTime))
        {
            JumpReset();
        }
    }

    //Reset Jump to init Values
    void JumpReset()
    {
        spaceTime = 0;
        playerState = PLAYER_STATUS.AIR;
        canJump = false;
    }

    //Produce the effect of coyote time
    private void CoyoteTime()
    {
        if (isGrounded) //Check if is grounded after do Coyote Time
        {
            coyoteTimeCounter = coyoteTime; //Here the Coyote Time is set up and ready to start deacrising
        }
        else
        {
            if (coyoteTimeCounter > 0f)
            {
                coyoteTimeCounter -= Time.deltaTime; //When you are not in ground, the Coyote time will start reducing, until you don't have more time to jump
            }
        }
    }

    //Make the action of ground hit
    private void GroundHit()
    {
        if (playerState == PLAYER_STATUS.AIR || playerState == PLAYER_STATUS.WALL)
        {
            if (jumpKeyHold && impactHitHold && hasImpactHit && !isCrouch && rb.linearVelocity.y < 0) //All check outs in term to do ground hit, only will effectuate if it's falling the player
            {
                hasImpactHit = false;
                isImpactHitting = true;
                rb.AddForce(new Vector2(0, -impactHit)); //Force to go down when you are in AIR
            }
        }

        //if(!isImpactHitting && !hasImpactHit) //Return ImpactHit
        //{
        //    if(jumpKeyUp) //This is to cancel multi doing Impact Hit if you don't key up Space or S
        //    {
        //        hasImpactHit = true;
        //        isImpactHitting = false;
        //    }
        //}
    }

    //Make the action of dashing
    private void Dash()
    { 
        if (canDash && !isRoof && !isCrouch)
        {
            if (dashDown && !isImpactHitting && !impactHitDown && playerFaceDir != PLAYER_FACE_DIRECTION.DOWN) //Check if the Left Shift is pressed, is in ground and is not sliding
            {
                dashFacing = playerFaceDir;
                //rb.AddForce(new Vector2(dashForce * playerDir, 0)); //Dash Force Input to Player, based on the face direction
                isDashing = true;
                canDash = false;
            }
        }
        else
        {
            if (isDashing)
            {
                dashTime += Time.deltaTime;
            }
            else
            {
                cooldownDashTime += Time.deltaTime;
            }

            if(dashTime >= dashDuration && isDashing) // If max time is passed, cancell dash and return player to maxSpeed
            {
                if(dashFacing == PLAYER_FACE_DIRECTION.UP && unlockAirDash)
                {
                    rb.linearVelocity = new Vector2(0, maxSpeedX);
                }
                else
                {
                    rb.linearVelocity = new Vector2(maxSpeedX * playerDir, 0);
                }

                rb.gravityScale = gravityEffect;
                isDashing = false;
                dashTime = 0;
            }

            if (dashFacing != playerFaceDir && isDashing) // If you change face direction cancell dash
            {
                DashCancell();
                //rb.velocity -= rb.velocity * 2 * Time.deltaTime;
                //rb.velocity -= rb.velocity * 2 * Time.deltaTime;
            }

            if (cooldownDashTime >= dashCooldown && !isDashing) //Wait for cooldown to do another dash
            {
                if (isGrounded || isHangingWall)
                {
                    canDash = true; //Reactivate dash
                    cooldownDashTime = 0;
                }
            }
            else
            {
                if (isDashing)
                {
                    playerState = PLAYER_STATUS.DASH; // Put player status in DASH
                    rb.gravityScale = 0; // Here we quit gravity when dashing, and now you can do Dashes in AIR

                    if (unlockAirDash) //If player Unlock Air dash we have to make distinctions between directions
                    {
                        if (dashFacing == PLAYER_FACE_DIRECTION.LEFT || dashFacing == PLAYER_FACE_DIRECTION.RIGHT)
                        {
                            rb.linearVelocity = new Vector2(maxSpeedX * playerDir + dashForce * playerDir * Time.fixedDeltaTime, 0); //Here is where we impulse the player to have more velocity, is more than maxSpeed but is slightly accelerating
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Here quit Y velocity movement to not go UP in dashes
                        }
                        else if (dashFacing == PLAYER_FACE_DIRECTION.UP)
                        {
                            rb.linearVelocity = new Vector2(0, maxSpeedX * playerDir + dashForce / 2 * playerDir * Time.fixedDeltaTime); //Here is where we impulse the player to have more velocity, is more than maxSpeed but is slightly accelerating
                            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Here quit Y velocity movement to not go UP in dashes
                        }
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(maxSpeedX * playerDir + dashForce * playerDir * Time.fixedDeltaTime, 0); //Here is where we impulse the player to have more velocity, is more than maxSpeed but is slightly accelerating
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Here quit Y velocity movement to not go UP in dashes
                    }
                }
                else
                {
                    rb.gravityScale = gravityEffect;
                }
            }
        }
    }

    public void DashCancell()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = gravityEffect;
        isDashing = false;
        dashTime = 0;
    }

    //Check the action of crouching and the state of it
    private void CrouchingGroundAndAir()
    {
        if (crouchHold && !isDashing)
        {
            isCrouch = true;

            UpCrouchCollider.enabled = true;

            playerUpCollider.enabled = false;
        }
        else
        {
            isCrouch = false;
        }

        if (isCrouch)
        {
            if (playerState == PLAYER_STATUS.GROUND) //If player is in GROUND, only reduce movility and upper box collider deactivate
            {
                playerState = PLAYER_STATUS.CROUCH;

                if (!DownCrouchCollider.isTrigger)
                {
                    playerSprite.sprite = Player_Down; //Set player sprite to Down
                    player_Material.color = new Color(0, 0, 256);
                    UpCrouchCollider.isTrigger = true;
                }
                else //In case player is in ground after crouching in AIR, return him ground
                {
                    if (earringFloor != 0 || isSlide) //Check if player will fall crouch into a ramp, if it is the case, can cause trouble with the underground issue, and fall off
                    {
                        DownCrouchCollider.isTrigger = false;

                        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + DownCrouchCollider.radius, this.gameObject.transform.position.z);
                        moveStopper = false;
                        isUnderground = false;
                    }
                    else //In case the player is crouched on air, and touch ground (not ramp angle), will return him upward off the ground when release Left_Control
                    {
                        moveStopper = true;
                        isUnderground = true;
                    }
                }
            }
            else if (playerState == PLAYER_STATUS.AIR) //If player is in AIR, only down circle collider deactivate
            {
                playerState = PLAYER_STATUS.CROUCH;

                playerSprite.sprite = Player_Up; //Set player sprite to Up
                player_Material.color = new Color(0, 256, 0);
                DownCrouchCollider.isTrigger = true;
                UpCrouchCollider.isTrigger = false;
            }
        }

        isRoof = Physics2D.OverlapAreaAll(RoofDetector.bounds.min, RoofDetector.bounds.max, roofMask).Length > 0; //Here we check if is a roof on top of the player

        if (isRoof) //In case is a roof on top of player, te state will mantain crouched
        {
            playerSprite.sprite = Player_Down; //Set player sprite to Down
            player_Material.color = new Color(0, 0, 256);
            UpCrouchCollider.isTrigger = true;
            playerState = PLAYER_STATUS.CROUCH;
            isCrouch = true;
        }

        if (!isCrouch) //Here we return to the not crouch state
        {
            if (isUnderground) //Here the player in case is underground, will pop up upward in ground
            {
                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + DownCrouchCollider.radius, this.gameObject.transform.position.z);
                moveStopper = false;
                isUnderground = false;
            }

            playerSprite.sprite = Player_Full; //Set player sprite to Full
            player_Material.color = new Color(256, 256, 256);
            UpCrouchCollider.isTrigger = true;
            DownCrouchCollider.isTrigger = false;

            playerUpCollider.enabled = true;

            if (playerState != PLAYER_STATUS.WALL && playerState != PLAYER_STATUS.HANGED)
            {
                jumpStopper = false; //Here we unblock the jump when you are crouch
            }
        }
        else
        {
            jumpStopper = true; //Here we block the jump when you are crouch
        }
    }

    //Check if Player is touching Layer Ground or Slide Ramp
    void CheckGroundAndSlide()
    {
        //Check if is Slide Ramp
        isSlide = Physics2D.OverlapAreaAll(GroundCheck.bounds.min, GroundCheck.bounds.max, slideMask).Length > 0;

        //Check if is Ground
        isGrounded = Physics2D.OverlapAreaAll(GroundCheck.bounds.min, GroundCheck.bounds.max, groundMask).Length > 0;

        //Check if is Ground
        if ((isGrounded && playerState != PLAYER_STATUS.JUMP) || (isGrounded && rb.linearVelocity.y == 0 && playerState == PLAYER_STATUS.JUMP)) //Check if the player was not actually jumping when you touch ground, this is because jumping to ground close to walls in corners, cause problems...
        {
            playerState = PLAYER_STATUS.GROUND;
            hasImpactHit = false;
            maxAirJumps = 0;

            if (isImpactHitting) //Prevent bouncing when touching ground
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                combatScript.ImpactHit();
                isImpactHitting = false;
            }

            if(rb.linearVelocity.y <= 0) //Return jump
            {
                canJump = true;
            }
        }

        //Check if is Slide in Ramp
        if (isSlide)
        {
            playerState = PLAYER_STATUS.GROUND;
            canJump = true;
            hasImpactHit = false;

            //Check if is Slide and has ImpactHit
            if (isImpactHitting && Mathf.Abs(earringFloor) > minAngleSlide)
            {
                rb.AddForce(new Vector2(0, -impactHit / 2));
            }
            else if (Mathf.Abs(earringFloor) > minAngleSlide) 
            {
                rb.AddForce(new Vector2(0, -slidingForce));
            }
        }

        //Put player in AIR status if is not in ground and not Sliding in Ramp
        if (!isGrounded && playerState != PLAYER_STATUS.JUMP && playerState != PLAYER_STATUS.AIR && playerState != PLAYER_STATUS.WALL && !isImpactHitting && !isSlide && !isDashing && !isHangingEdge) //Also check to not do infinit ImpactHit
        {
            hasImpactHit = true;

            if(coyoteTimeCounter <= 0f) //Check if Coyote Time has passed to cancel Jump
            {
                playerState = PLAYER_STATUS.AIR;

                if (unlockDoubleJump && maxAirJumps < 2)
                {
                    canJump = true;
                }
                else
                {
                    canJump = false;
                }
            }
        }

        //Check ground to return canUnhang to edges
        if (isGrounded && canUnhang)
        {
            canUnhang = false;
        }
    }

    //Check for walls to hang and do the action
    void CheckWalls()
    {
        //Check if is Left Wall
        isLeftWall = Physics2D.OverlapAreaAll(LeftWallCheck.bounds.min, LeftWallCheck.bounds.max, wallMask).Length > 0;

        //Check if is Right Wall
        isRightWall = Physics2D.OverlapAreaAll(RightWallCheck.bounds.min, RightWallCheck.bounds.max, wallMask).Length > 0;

        //If you are hanged and jump, unfreeze player to move again
        if (playerState == PLAYER_STATUS.JUMP && isHangingWall)
        {
            PlayerUnFrezze();

            rb.AddForce(new Vector2(hangWallImpulseSides * (isRightWall ? 1 : -1), hangWallImpulseUp)); //Extra impulse to get out of the wall
        }

        //Check if is not touching walls
        if (!isLeftWall && !isRightWall)
        {
            isHangingWall = false;
        }

        //Check if is touching one wall and if is not already hanged, and then freeze player, also check if is not impact hitting, lastly if you are dashing when hit wall cancell it
        if ((isRightWall || isLeftWall) && !isHangingWall && (playerState == PLAYER_STATUS.JUMP || playerState == PLAYER_STATUS.AIR || (playerState == PLAYER_STATUS.DASH && !isGrounded)) && !isImpactHitting)
        {
            if(playerState == PLAYER_STATUS.DASH)
            {
                isDashing = false;
                canDash = true; //Reactivate dash
                rb.gravityScale = gravityEffect;
                dashTime = 0;
            }

            PlayerFrezze();
            JumpReset();
            playerState = PLAYER_STATUS.WALL;
            isHangingWall = true;
            canJump = true;
            maxAirJumps = 0;

            //EMERGENCY JUMP STOP! Here the player will stop jumping when touching wall and prevent from deataching without wanting, and falling off
            if (jumpKeyHold && !jumpStopper)
            {
                jumpStopper = true;
            }
        }

        if(playerState == PLAYER_STATUS.AIR && rb.constraints == RigidbodyConstraints2D.FreezePosition && !isHangingEdge)
        {
            PlayerUnFrezze();
            canJump = true;
            maxAirJumps = 0;
        }
    }

    //Freeze player position
    void PlayerFrezze() 
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    //Frezze player rotation
    void PlayerUnFrezze() 
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //this.gameObject.transform.rotation = new Quaternion(0,0,0,1);
    }

    //Check the player facing direction
    void CheckPlayerFaceDirection()
    {
        if (rightKey)
        {
            playerFaceDir = PLAYER_FACE_DIRECTION.RIGHT;

            if (upKey)
            {
                playerFaceDir = PLAYER_FACE_DIRECTION.RIGHT_UP;
            }

            if (downKey)
            {
                playerFaceDir = PLAYER_FACE_DIRECTION.RIGHT_DOWN;
            }
        }
        
        if (leftKey)
        {
            playerFaceDir = PLAYER_FACE_DIRECTION.LEFT;

            if (upKey)
            {
                playerFaceDir = PLAYER_FACE_DIRECTION.LEFT_UP;
            }

            if (downKey)
            {
                playerFaceDir = PLAYER_FACE_DIRECTION.LEFT_DOWN;
            }
        }

        if (upKey)
        {
            playerFaceDir = PLAYER_FACE_DIRECTION.UP;
        }
        
        if (downKey)
        {
            playerFaceDir = PLAYER_FACE_DIRECTION.DOWN;
        }

        switch (playerFaceDir)
        {
            case PLAYER_FACE_DIRECTION.UP:
                if (unlockAirDash)
                {
                    playerDir = 1;
                }
                break;
            case PLAYER_FACE_DIRECTION.DOWN:
                break;
            case PLAYER_FACE_DIRECTION.LEFT:
                playerDir = -1;
                flipAnimation = true;
                break;
            case PLAYER_FACE_DIRECTION.RIGHT:
                playerDir = 1;
                flipAnimation = false;
                break;
            case PLAYER_FACE_DIRECTION.RIGHT_UP:
                break;
            case PLAYER_FACE_DIRECTION.RIGHT_DOWN:
                break;
            case PLAYER_FACE_DIRECTION.LEFT_UP: 
                break;
            case PLAYER_FACE_DIRECTION.LEFT_DOWN:
                break;
            default: 
                break;
        }
    }

    //Add the baisc movement force to the player
    void AddMovementSpeed()
    {
        //EMERGENCY Stopper for Movement
        if (!moveStopper)
        {
            if (Mathf.Abs(rb.linearVelocity.x) < (!isCrouch ? maxSpeedX : (maxSpeedX / crouchSpeedReduction)) && !isDashing) //Chech for Max Speed not been overpassed (Also if is crouched maxSpeed is divided / crouchSpeedReduction), also if you are Dashing dont matter you maxSpeed because will be overpassed too
            {
                if (!isCrouch) //Check if player is not CROUCH
                {
                    //Add movement Speed!
                    if (isGrounded)
                    {
                        rb.linearVelocity += move * speed * Time.deltaTime; //Movement in floor
                    }
                    else
                    {
                        rb.linearVelocity += move * speed / airSpeedReduction * Time.deltaTime; //Movement in air is reduced
                    }
                }
                else
                {
                    //Reduce movement if is CROUCH
                    if (isGrounded)
                    {
                        rb.linearVelocity += move * (speed / (isRoof ? (crouchSpeedReduction / 2f) : crouchSpeedReduction)) * Time.deltaTime; //Movement in floor crouched
                    }
                    else
                    {
                        rb.linearVelocity += move * (speed / (crouchSpeedReduction * multiplierAirCrouch)) * Time.deltaTime; //Movement in air crouched is more reduced
                    }

                    if(move.x == 0 && rb.linearVelocity.x != 0) //This is for not slide to much when you are crouched by innerthia
                    {
                        if(rb.linearVelocity.x > 0)
                        {
                            rb.linearVelocity -= new Vector2(speed * Time.deltaTime, 0); //In case you go Right
                        }
                        else if(rb.linearVelocity.x < 0)
                        {
                            rb.linearVelocity += new Vector2(speed * Time.deltaTime, 0); //In case you go Left
                        }
                    }
                }
            }
            else
            {
                if (playerState == PLAYER_STATUS.JUMP || playerState == PLAYER_STATUS.AIR) //In case player has overpassed max velocity in air or jumping --> reduce it
                {
                    if (rb.linearVelocity.x > 0)
                    {
                        rb.linearVelocity -= new Vector2((speed / airSpeedReduction) * Time.deltaTime, 0); //In case you go Right
                    }
                    else if (rb.linearVelocity.x < 0)
                    {
                        rb.linearVelocity += new Vector2((speed / airSpeedReduction) * Time.deltaTime, 0); //In case you go Left
                    }
                }
            }

            if (rb.linearVelocity.y < -maxSpeedY) //Max reach velocity at Y when falling
            {
                if (isImpactHitting)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxSpeedY * 2);
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxSpeedY);
                }
            }
        }

        if(move.x == 0 && isGrounded && !isSlide && !isRoof && !isDashing) //Movement Reduction (fictional reduction)
        {
            if (rb.linearVelocity.x > 1f)
            {
                rb.linearVelocity -= new Vector2(speed / icedFloorEffect * Time.deltaTime, 0);
            }
            else if (rb.linearVelocity.x < -1f)
            {
                rb.linearVelocity += new Vector2(speed / icedFloorEffect * Time.deltaTime, 0);
            }
        }

        if (move.x == 0 && !isGrounded && !isSlide && !isRoof && !isDashing) //Movement Air Reduction (fictional reduction)
        {
            if (rb.linearVelocity.x > 1f)
            {
                rb.linearVelocity -= new Vector2(speed / airSpeedReduction * Time.deltaTime, 0);
            }
            else if (rb.linearVelocity.x < -1f)
            {
                rb.linearVelocity += new Vector2(speed / airSpeedReduction * Time.deltaTime, 0);
            }
        }
    }

    //Here we check if the player angle between the floor is too much or correct in order to jump or do things
    void CheckEarringFloor()
    {
        RaycastHit2D hitDownEarringGround = Physics2D.Raycast(transform.position, Vector2.down, (transform.localScale.y / 2) + 1f, groundMask); //Debug ray to check the ground
        RaycastHit2D hitDownEarringSlide = Physics2D.Raycast(transform.position, Vector2.down, (transform.localScale.y / 2) + 1f, slideMask); //Debug ray to check the slide floor

        //Debugs ray's
        //Debug.DrawRay(transform.position, Vector2.down * ((transform.localScale.y / 2) + 1f), Color.red);
        //Debug.DrawRay(hitDownEarringSlide.point, new Vector2(1, 0), Color.blue);
        //Debug.DrawRay(hitDownEarringSlide.point, hitDownEarringSlide.normal, Color.green);


        //Here we check if the angle is more than the maxAngleFloor
        if (hitDownEarringGround && isGrounded)
        {
            //Calculate the angle between the floor on earringFloor
            earringFloor = Vector2.Angle(hitDownEarringGround.normal, Vector2.up); //Here we take the raw angle without sign of the earrings
            earringFloor = earringFloor * Mathf.Sign(hitDownEarringGround.transform.rotation.z); //Here we put the sign

            if (earringFloor != 0 && Mathf.Abs(earringFloor) >= maxAngleFloor)
            {
                isTooMuchEarring = true;
            }
        }
        else if(hitDownEarringSlide && isSlide)
        {
            //Calculate the angle between the floor on earringFloor
            earringFloor = Vector2.Angle(hitDownEarringSlide.normal, Vector2.up); //Here we take the raw angle without sign of the earrings
            earringFloor = earringFloor * Mathf.Sign(hitDownEarringSlide.transform.rotation.z); //Here we put the sign

            if (earringFloor != 0 && Mathf.Abs(earringFloor) >= maxAngleFloor)
            {
                isTooMuchEarring = true;
            }
        }
        else
        {
            earringFloor = 0; //In case of is not in slide or ground, put the angle to 0
        }

        //In case earringFloor is not more than maxAngleFloor, player will continue able Jumping
        if (Mathf.Abs(earringFloor) < maxAngleFloor)
        {
            isTooMuchEarring = false;
        }
    }

    //Animates de player for movement
    void AnimatePlayer()
    {
        if (flipAnimation) //Flip the animation if it is necesary
        {
            playerSprite.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;
        }

        if(playerState != PLAYER_STATUS.CROUCH)
        {
            if(animator.enabled == false)
            {
                animator.enabled = true;
            }

            if (rb.linearVelocity.magnitude > 0.1f && move.x != 0 && move.y == 0 && playerState == PLAYER_STATUS.GROUND)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            }

            //Set an animation of jumping, and falling from jump
            if (rb.linearVelocity.y > 0f && (playerState == PLAYER_STATUS.AIR || playerState == PLAYER_STATUS.JUMP))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Jump", true);
            }
            else if (rb.linearVelocity.y <= 0f && playerState == PLAYER_STATUS.AIR)
            {
                animator.SetBool("Jump", false);
            }

            //Idle is to return from jumping and wait to touch ground
            if (!animator.GetBool("Jump") && !animator.GetBool("Run"))
            {
                if ((playerState == PLAYER_STATUS.GROUND || playerState == PLAYER_STATUS.JUMP) && !animator.GetBool("Idle"))
                {
                    animator.SetBool("Idle", true);
                }
            }
        }
        else
        {
            animator.enabled = false;
        }
    }
}
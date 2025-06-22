using UnityEngine;
using UnityEngine.Rendering;
using PlayerController;
using UnityEngine.UI;

public class HangEdges : MonoBehaviour
{
    public Transform playerFinalPosition;
    private GameObject player;
    public bool isHanged;
    public bool moveToNewPosition;
    public float speedTransform;

    bool playerCollision;
    bool finishClimb;
    CharacterPlayerController characterPlayerController;

    //Indicate which edge is goind to be
    [Header("Which edge is?")]
    public bool isLeftEdge;
    public bool isRightEdge;

    [Header("Gameobject Colliders")]
    public BoxCollider2D leftEdgeCollider;
    public BoxCollider2D rightEdgeCollider;

    [Header("Player First Position at collision")]
    public Transform playerPositionLeft;
    public Transform playerPositionRight;

    [Header("Edge Mask")]
    public LayerMask playerEdgeMask;

    float climbCooldown;
    float hangCooldown;

    private void Start()
    {
        isHanged = false;
        finishClimb = false;
        climbCooldown = 0f;

        if (isLeftEdge)
        {
            leftEdgeCollider.gameObject.SetActive(true);
            rightEdgeCollider.gameObject.SetActive(false);
            playerPositionLeft.gameObject.SetActive(true);
            playerPositionRight.gameObject.SetActive(false);
        }

        if (isRightEdge)
        {
            rightEdgeCollider.gameObject.SetActive(true);
            leftEdgeCollider.gameObject.SetActive(false);
            playerPositionRight.gameObject.SetActive(true);
            playerPositionLeft.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (climbCooldown > 0f)
        {
            isHanged = false;
            climbCooldown -= Time.deltaTime;
        }

        if (isHanged) //Once the player is hanged
         {
             if (characterPlayerController.canUnhang && characterPlayerController.climbEdges) //If the player has released the space button and the climb edge key is pressed move the player to the new position
             {
                 moveToNewPosition = true;
                 isHanged = false;
                 climbCooldown = 0.2f;
             }
         }

        if (moveToNewPosition) //Here is where the player will be moved to the exit position
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerFinalPosition.position, speedTransform * Time.deltaTime); //The function that moves the player

            characterPlayerController.jumpStopper = true;

            if (Vector3.Distance(player.transform.position, playerFinalPosition.position) < 0.1f || Vector3.Distance(player.transform.position, playerFinalPosition.position) > 5f) //If the player is close enought or to far away exit the hang situation
            {
                characterPlayerController.rb.linearVelocity = Vector2.zero;

                if(!characterPlayerController.jumpKeyHold) //Check that the player is not pressing the jump button to not make an involuntary jump when ending the climb
                {
                    characterPlayerController.jumpStopper = false;
                    characterPlayerController.playerOnEdgeUnfreeze = true;
                    finishClimb = false;
                    moveToNewPosition = false;
                }
            }
        }

        if (characterPlayerController != null)
        {
            if (isHanged && (characterPlayerController.playerState == CharacterPlayerController.PLAYER_STATUS.GROUND || characterPlayerController.playerState == CharacterPlayerController.PLAYER_STATUS.WALL))
            {
                finishClimb = false;
                isHanged = false;
            }
        }

        if (isLeftEdge)
        {
            playerCollision = Physics2D.OverlapAreaAll(leftEdgeCollider.bounds.min, leftEdgeCollider.bounds.max, playerEdgeMask).Length > 0;
        }

        if(isRightEdge)
        {
            playerCollision = Physics2D.OverlapAreaAll(rightEdgeCollider.bounds.min, rightEdgeCollider.bounds.max, playerEdgeMask).Length > 0;
        }

        if(playerCollision && !isHanged && !finishClimb)
        {
            characterPlayerController = GameObject.Find("Player").GetComponent<CharacterPlayerController>();

            if (characterPlayerController.enabled)
            {
                characterPlayerController.isHangingEdge = true;
                player = characterPlayerController.gameObject;

                if(isLeftEdge)
                {
                    characterPlayerController.gameObject.GetComponent<AnimationManager>().isLeftEdge = true;
                }
                else
                {
                    characterPlayerController.gameObject.GetComponent<AnimationManager>().isLeftEdge = false;
                }

                if (isLeftEdge)
                {
                    player.transform.position = playerPositionLeft.transform.position;
                }

                if (isRightEdge)
                {
                    player.transform.position = playerPositionRight.transform.position;
                }

                finishClimb = true;
                isHanged = true;
            }
        }

        if (finishClimb)
        {
            if (characterPlayerController.dropDown || characterPlayerController.downControllerSensitivity < -0.8f)
            {
                finishClimb = false;
                hangCooldown = 0.3f;
            }
        }

        if (isHanged && hangCooldown > 0.01f)
        {
            hangCooldown -= Time.deltaTime;
        }
        else
        {
            if(hangCooldown >= 0)
            {
                isHanged = false;
                hangCooldown = -1f;
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Edge"))
    //    {
    //        playerController = collision.transform.parent.parent.GetComponent<CharacterPlayerController>();

    //        playerController.isHangingEdge = true;
    //        player = collision.transform.parent.parent.gameObject;

    //        if (isLeftEdge)
    //        {
    //            player.transform.position = playerPositionLeft.transform.position;
    //        }

    //        if (isRightEdge)
    //        {
    //            player.transform.position = playerPositionRight.transform.position;
    //        }

    //        isHanged = true;
    //    }
    //}
}

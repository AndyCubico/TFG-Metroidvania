using UnityEngine;
using static PlayerController.CharacterPlayerController;
using UnityEngine.Playables;
using PlayerController;

public class AnimationManager : MonoBehaviour
{
    public PlayerCombatV2 playerCombat;
    public HeavyAttack heavyAttack;
    private CharacterPlayerController characterPlayerController;
    private Animator animator;

    [HideInInspector] public bool isLeftEdge;

    private void Start()
    {
        characterPlayerController = GetComponent<CharacterPlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Player animations manager
        AnimatePlayer();
    }

    public void BasicAttackCombatAnimationHasEnded()
    {
        playerCombat.AnimationHasFinished();
    }

    public void StartBasicAttack()
    {
        playerCombat.StartAttacking();
    }

    public void HeavyAttackHasEnded()
    {
        heavyAttack.AnimationHasFinished();
    }

    public void HeavyAttackHit()
    {
        heavyAttack.Hit();
    }

    //Animates de player for movement
    void AnimatePlayer()
    {
        if (characterPlayerController.flipAnimation && !characterPlayerController.blockFlip) //Flip the animation if it is necesary
        {
            characterPlayerController.playerSprite.flipX = true;
        }
        else if (!characterPlayerController.blockFlip)
        {
            characterPlayerController.playerSprite.flipX = false;
        }

        if (characterPlayerController.playerState != PLAYER_STATUS.CROUCH)
        {
            if (animator.enabled == false)
            {
                animator.enabled = true;
            }

            if ((characterPlayerController.rb.linearVelocity.magnitude > 0.1f || characterPlayerController.move.x != 0) && characterPlayerController.move.y == 0 && characterPlayerController.playerState == PLAYER_STATUS.GROUND)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Edge", false);
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            }

            //Set an animation of jumping, and falling from jump
            if (characterPlayerController.rb.linearVelocity.y > 0f && (characterPlayerController.playerState == PLAYER_STATUS.AIR || characterPlayerController.playerState == PLAYER_STATUS.JUMP))
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Jump", true);
            }
            else if (characterPlayerController.rb.linearVelocity.y <= 0f && characterPlayerController.playerState == PLAYER_STATUS.AIR)
            {
                animator.SetBool("Jump", false);
            }

            //Idle is to return from jumping and wait to touch ground
            if (!animator.GetBool("Jump") && !animator.GetBool("Run"))
            {
                if ((characterPlayerController.playerState == PLAYER_STATUS.GROUND || characterPlayerController.playerState == PLAYER_STATUS.JUMP) && !animator.GetBool("Idle"))
                {
                    animator.SetBool("Idle", true);
                    animator.SetBool("Edge", false);
                }
            }

            //Exit from jump when hang edging
            if (animator.GetBool("Jump") && characterPlayerController.playerState == PLAYER_STATUS.GROUND)
            {
                animator.SetBool("Idle", true);
                animator.SetBool("Edge", false);
                animator.SetBool("Jump", false);
            }

            //Hang walls
            if (characterPlayerController.playerState == PLAYER_STATUS.WALL)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Run", false);
                animator.SetBool("Edge", false);
                //animator.SetBool("Dash", false);

                characterPlayerController.blockFlip = true;
                animator.SetBool("Wall", true);

                if (characterPlayerController.isLeftWall)
                {
                    characterPlayerController.playerSprite.flipX = true;
                }
                else if (characterPlayerController.isRightWall)
                {
                    characterPlayerController.playerSprite.flipX = false;
                }
            }

            //Get out of hang walls
            if (animator.GetBool("Wall") && (characterPlayerController.playerState == PLAYER_STATUS.JUMP || characterPlayerController.playerState == PLAYER_STATUS.AIR || characterPlayerController.playerState == PLAYER_STATUS.GROUND))
            {
                characterPlayerController.blockFlip = false;
                animator.SetBool("Wall", false);

                if (characterPlayerController.playerState == PLAYER_STATUS.JUMP)
                {
                    animator.SetBool("Jump", true);
                }
                else if (characterPlayerController.playerState == PLAYER_STATUS.AIR)
                {
                    animator.SetBool("Jump", false);
                }
            }

            if (characterPlayerController.playerState == PLAYER_STATUS.HANGED)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Jump", false);
                animator.SetBool("Run", false);
                animator.SetBool("Wall", false);
                //animator.SetBool("Dash", false);

                characterPlayerController.blockFlip = false;

                if (isLeftEdge)
                {
                    characterPlayerController.playerSprite.flipX = false;
                }
                else
                {
                    characterPlayerController.playerSprite.flipX = true;
                }

                if (characterPlayerController.climbEdges || characterPlayerController.playerState == PLAYER_STATUS.GROUND)
                {
                    animator.SetBool("Edge", false);
                }
                else
                {
                    animator.SetBool("Edge", true);
                }
            }
        }
        else
        {
            animator.enabled = false;
        }
    }
}

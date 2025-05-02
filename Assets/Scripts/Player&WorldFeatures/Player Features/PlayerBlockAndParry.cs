using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlockAndParry : MonoBehaviour
{
    [Header("Times")]
    [Space(5)]
    bool isBlocking;
    public float blockCooldown;
    float blockTimer;

    public float maxParryTime;
    float parryCounter;

    public float invincibilityTime;
    float invincibilityCounter;

    [Header("Receive Damages")]
    [Space(5)]
    public float damageBlock;
    public float damageNoBlock;


    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference BlockAndParryAction;

    [Header("Testing")]
    [Space(5)]
    public bool enemyTesting;
    SpriteRenderer enemyTest;

    // Bool to check if an enemy attack has hitted
    bool enemyIsAttacking;

    // Player Rigidbody
    public Rigidbody2D rb;

    // Player Controller
    public CharacterPlayerController characterPlayerController;

    // Controls Action Input Delegates
    private void OnEnable()
    {
        BlockAndParryAction.action.started += BlockAndParryEvent;
    }

    private void OnDisable()
    {
        BlockAndParryAction.action.started -= BlockAndParryEvent;
    }

    public void BlockAndParryEvent(InputAction.CallbackContext context)
    {
        if(characterPlayerController.playerState == CharacterPlayerController.PLAYER_STATUS.GROUND || characterPlayerController.playerState == CharacterPlayerController.PLAYER_STATUS.AIR)
        {
            if (blockTimer <= 0f) // Timer to be able to block again
            {
                if (context.performed)
                {
                    isBlocking = true;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                }

                if (context.canceled)
                {
                    isBlocking = false;
                    blockTimer = blockCooldown; // When the block is quit the timer is applied
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
    }

    void Start()
    {
        isBlocking = false;
    }

    void Update()
    {
        if(blockTimer > 0f) // In case of have block, the cooldown will happen
        {
            blockTimer -= Time.deltaTime;
        }

        if (isBlocking) // The parry starts counting from the moment the player has blocked
        {
            parryCounter += Time.deltaTime;
        }
        else
        {
            parryCounter = 0; // The parry time returns to 0 at the moment of stop blocking
        }

        if(invincibilityCounter <= 0f) // When the player receives damage, blocks or parries a little invincibility will happen to different contemplated situations
        {
            if (enemyIsAttacking) // If an enemy attack is hitting on the player
            {
                if (isBlocking) // If the block action is executed correctly
                {
                    if (parryCounter > 0 && parryCounter <= maxParryTime) // If the time of parry is available
                    {
                        ReciveAttackParryWindow(); // Call the function of parry
                    }
                    else
                    {
                        ReciveAttackBlockWindow(); // Call the function of block
                    }
                }
                else
                {
                    PlayerHasBeenHitted(); // Player has been hitted without blocking or parrying
                }
            }
        }
        else
        {
            invincibilityCounter -= Time.deltaTime; // Invincibility timer starts reducing
        }
    }

    // Execute a parry
    void ReciveAttackParryWindow()
    {
        enemyIsAttacking = false;

        //Send that the parry has been done correcltly

        ActiveInvincibility(); // Active the invencibility

        if (enemyTesting) // Enemy color testing
        {
            enemyTest.color = Color.green;
        }
    }

    // Execute a Block
    void ReciveAttackBlockWindow()
    {
        enemyIsAttacking = false;

        // The attack has been blocked

        if (enemyTesting) // Enemy color testing
        {
            enemyTest.color = Color.blue;
        }
    }

    // Execute damage to the player
    void PlayerHasBeenHitted()
    {
        enemyIsAttacking = false;

        // Damage player

        ActiveInvincibility(); // Active the invincibility

        if (enemyTesting) // Enemy color testing
        {
            enemyTest.color = Color.black;
        }
    }

    // Enables invincibility
    void ActiveInvincibility()
    {
        invincibilityCounter = invincibilityTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack")) // If the type of collision is "EnemyAttack"
        {
            enemyIsAttacking = true; // Active the bool of attacking

            if (enemyTesting) //Take the enemy sprite renderer if is on testing
            {
                enemyTest = collision.gameObject.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack")) // If the type of collision is "EnemyAttack"
        {
            enemyIsAttacking = false; // Deactivate the enemy attack
        }
    }
}

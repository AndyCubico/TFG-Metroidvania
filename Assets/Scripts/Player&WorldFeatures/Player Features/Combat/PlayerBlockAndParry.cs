using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public static class HealthEvents
{
    public static Action<float> TakingDamage;
}

public class PlayerBlockAndParry : MonoBehaviour
{
    [Header("Times")]
    [Space(5)]
    [HideInInspector] public bool isBlocking;
    public float blockCooldownTime;
    float blockCooldownCounter;

    float blockCounter;
    public float blockTime;

    public float maxParryTime;
    float parryCounter;

    public float invincibilityTime;
    float invincibilityCounter;
    bool isInvencible;
    bool isDashing;

    [Header("Receive Damages")]
    [Space(5)]
    public float damageBlockPercentage;
    float damageBlock;
    float damageNoBlock;
    bool canAttackBeParried;

    [Header("Extra variables")]
    [Space(5)]
    public float hittedForce;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference BlockAndParryAction;

    [Header("Testing")]
    [Space(5)]
    public bool enemyTesting;
    SpriteRenderer enemyTest;

    // Bool to check if an enemy attack has hitted
    bool enemyIsAttacking;

    // Controls from where the attack is comming
    int attackDirection;

    // Player Rigidbody
    public Rigidbody2D rb;

    // Player Controller
    public CharacterPlayerController characterPlayerController;

    // Heavy Attack
    public HeavyAttack heavyAttack;

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
            if (blockCooldownCounter <= 0f) // Timer to be able to block again
            {
                if (context.performed)
                {
                    isBlocking = true;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                }

                if (context.canceled)
                {
                //    isBlocking = false;
                //    blockTimer = blockCooldown; // When the block is quit the timer is applied
                //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
    }

    void Start()
    {
        isBlocking = false;
        canAttackBeParried = false;
        parryCounter = 0f;
        blockCounter = 0f;
        invincibilityCounter = 0f;
        attackDirection = 0;
    }

    void Update()
    {
        if (blockCooldownCounter > 0f) // In case of have block, the cooldown will happen
        {
            blockCooldownCounter -= Time.deltaTime;
        }

        if (isBlocking) // The parry starts counting from the moment the player has blocked
        {
            if(parryCounter <= maxParryTime && canAttackBeParried) // Check if the time for parry is still open and the attack can be parried
            {
                parryCounter += Time.deltaTime;
            }
            else if(blockCounter <= blockTime) // Check if the parry apperture has passed and the block time stills open
            {
                blockCounter += Time.deltaTime;
            }
            else
            {
                ResetBlock(); // Reset the block when ends the process
            }
        }

        if (characterPlayerController.isDashing)
        {
            isDashing = true;
        }
        else
        {
            isDashing = false;
        }

        if (!isInvencible && !isDashing) // When the player receives damage, blocks or parries a little invincibility will happen to different contemplated situations
        {
            if (enemyIsAttacking) // If an enemy attack is hitting on the player
            {
                if (isBlocking) // If the block action is executed correctly
                {
                    if (parryCounter > 0 && parryCounter <= maxParryTime && canAttackBeParried) // Check if the time for parry is still open and the attack can be parried
                    {
                        ReciveAttackParryWindow(); // Call the function of parry
                        ResetBlock();
                    }
                    else
                    {
                        if(blockCounter > 0f && blockCounter <= blockTime)
                        {
                            ReciveAttackBlockWindow(); // Call the function of block
                            ResetBlock();
                        }
                        else
                        {
                            ResetBlock();
                        }
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

            if(invincibilityCounter <= 0)
            {
                isInvencible = false;
            }
        }
    }

    private void ResetBlock()
    {
        isBlocking = false;
        blockCounter = 0;
        parryCounter = 0;
        blockCooldownCounter = blockCooldownTime;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Execute a parry
    void ReciveAttackParryWindow()
    {
        enemyIsAttacking = false;
        heavyAttack.AddCharges(1);

        //Send that the parry has been done correcltly
        Debug.Log("Parry");

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
        Debug.Log(damageBlock);

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
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(attackDirection * hittedForce, hittedForce / 2), ForceMode2D.Impulse); // Do a force in the contrary direction of the attack
        HealthEvents.TakingDamage?.Invoke(damageNoBlock); // Call the delegate to recieve damage and cancell abilities
        Debug.Log(damageNoBlock);

        ActiveInvincibility(); // Active the invincibility

        if (enemyTesting) // Enemy color testing
        {
            enemyTest.color = Color.black;
        }
    }

    // Enables invincibility
    void ActiveInvincibility()
    {
        isInvencible = true;
        invincibilityCounter = invincibilityTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invincibilityCounter <= 0f) //No new attacks entry if player is invencible
        {
            if (collision.CompareTag("EnemyAttack")) // If the type of collision is "EnemyAttack"
            {
                //Check for the direction of the attack
                float distance = 0;
                distance = transform.position.x - collision.transform.position.x;

                if (distance > 0f)
                {
                    attackDirection = 1;
                }
                else if (distance < 0f)
                {
                    attackDirection = -1;
                }
                else
                {
                    attackDirection = 0;
                }

                EnemyHit enemyHit;

                // Recieve here the damage that the hit is going to make
                if(collision.TryGetComponent(out enemyHit))
                {
                    if (!enemyHit.hasHittedPlayer) // If the attack already does not hit the player
                    {
                        damageNoBlock = enemyHit.damage;
                        canAttackBeParried = enemyHit.canBeParried;
                        enemyHit.hasHittedPlayer = true;
                        damageBlock = (damageNoBlock * (100 - damageBlockPercentage)) / 100; // Damage blocking is a % of the total incoming damage, is calculated here

                        enemyIsAttacking = true; // Active the bool of attacking

                        if (enemyTesting) //Take the enemy sprite renderer if is on testing
                        {
                            enemyTest = collision.gameObject.GetComponent<SpriteRenderer>();
                        }
                    }
                    else
                    {
                        damageNoBlock = 0f;
                    }
                }
                else
                {
                    damageNoBlock = 0f;
                }
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

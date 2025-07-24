using PlayerController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialAbilities : MonoBehaviour
{
    [Header("Character Player Controller")]
    [Space(5)]
    public CharacterPlayerController characterPlayerController;

    [Header("Rigidbody")]
    [Space(5)]
    public Rigidbody2D rb;
    bool rigidbodyFreeze;

    [Header("- Habilities -")]

    [Space(5)]
    [Header("Snow")]
    public GameObject snowPrefab;
    public float snowPreparationTime;
    public float snowRecuperationTime;
    public float snowAttackCooldown;
    private float snowAttackTimer;
    public float downForce;
    public float snowDamage;
    public float snowExpansionSpeed;
    public float snowExpansionMaxSize;
    private bool snowExpand;
    private float sizeSnowExpansion;
    private GameObject snowGameObj;

    [Header("Colliders")]
    [Space(5)]
    public BoxCollider2D groundCollider;
    public BoxCollider2D snowCollider;

    [Header("Layers")]
    [Space(5)]
    public LayerMask groundLayer;
    public LayerMask abilityCancel;

    float defaultGravity;

    bool isGrounded;
    bool cancelLayers;
    bool isSnowAttacking;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference SpecialHabilitiesTrigger;
    public InputActionReference SnowHability;

    private List<IHittableObject> enemiesHealth;

    bool usingController;
    [HideInInspector]public bool specialHabilitiesTrigger;
    bool snowHability;

    private Coroutine activeRoutine;

    //Scripts
    private PlayerHealth m_PlayerHealth;
    private PlayerCombatV2 m_PlayerCombat;

    AttackFlagType attackFlagType = AttackFlagType.None;

    private void OnEnable()
    {
        SpecialHabilitiesTrigger.action.started += SpecialHabilitiesEvent;
        SnowHability.action.started += SnowHabilityEvent;
        HealthEvents.TakingDamage += ReceiveAnAttack;
    }

    private void OnDisable()
    {
        SpecialHabilitiesTrigger.action.started -= SpecialHabilitiesEvent;
        SnowHability.action.started -= SnowHabilityEvent;
        HealthEvents.TakingDamage -= ReceiveAnAttack;
    }

    public void SpecialHabilitiesEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            specialHabilitiesTrigger = true;
        }
        else
        {
            specialHabilitiesTrigger = false;
        }
    }

    public void SnowHabilityEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            snowHability = true;
        }
        else
        {
            snowHability = false;
        }
    }

    void Start()
    {
        m_PlayerCombat = this.transform.parent.GetComponent<PlayerCombatV2>();
        m_PlayerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();

        rigidbodyFreeze = false;
        isSnowAttacking = false;
        snowExpand = false;
        m_PlayerCombat.isAttacking = false;
        defaultGravity = rb.gravityScale;
        snowAttackTimer = 0;
        sizeSnowExpansion = 0;

        snowCollider.size = new Vector2(0, snowCollider.size.y);

    }

    void Update()
    {
        //Check if is there is something at LeftAttack
        isGrounded = Physics2D.OverlapAreaAll(groundCollider.bounds.min, groundCollider.bounds.max, groundLayer).Length > 0;
        cancelLayers = Physics2D.OverlapAreaAll(groundCollider.bounds.min, groundCollider.bounds.max, abilityCancel).Length > 0;

        //Check what type of controller the player is using
        if (Gamepad.current != null)
        {
            usingController = true;
        }
        else if (Keyboard.current != null)
        {
            usingController = false;
        }
        else
        {
            usingController = false;
        }

        if (!characterPlayerController.isDashing && !m_PlayerHealth.isHealing && !m_PlayerCombat.isAttacking && !characterPlayerController.isInWater) // If the player is dashing can't make an ability and is not healing
        {
            if (usingController)
            {
                //characterPlayerController.enabled = !specialHabilitiesTrigger;

                //if (specialHabilitiesTrigger && !rigidbodyFreeze)
                //{
                //    rb.linearVelocity = Vector3.zero;
                //    rigidbodyFreeze = true;
                //}
                //else if (!specialHabilitiesTrigger)
                //{
                //    rigidbodyFreeze = false;
                //}

                if (specialHabilitiesTrigger && snowHability && !isSnowAttacking && snowAttackTimer <= 0) // Check for the inputs be pressed and not be already doing  the attack, and the cooldown be zero
                {
                    snowAttackTimer = snowAttackCooldown;

                    m_PlayerCombat.isAttacking = true;
                    activeRoutine = StartCoroutine(SnowSpecialAttack());
                }
            }
            else
            {
                //if (isSnowAttacking /*More abilities*/)
                //{
                //    if (!rigidbodyFreeze)
                //    {
                //        rb.linearVelocity = Vector3.zero;
                //        rb.gravityScale = 0;
                //        rigidbodyFreeze = true;
                //    }
                //}

                if (snowHability && !isSnowAttacking && snowAttackTimer <= 0) // Check for the input be pressed and not be already doing  the attack, and the cooldown be zero
                {
                    snowAttackTimer = snowAttackCooldown;

                    m_PlayerCombat.isAttacking = true;
                    activeRoutine = StartCoroutine(SnowSpecialAttack());
                }
            }
        }

        //_Snow
        if (snowExpand) // If the snow attack is happening
        {
            //Collider Expansion
            sizeSnowExpansion += Time.deltaTime * snowExpansionSpeed;
            snowCollider.size = new Vector2(sizeSnowExpansion, snowCollider.size.y);

            (List<IHittableObject> enemyList, List<GameObject> enemyObjects) = snowCollider.gameObject.GetComponent<Attack_Detectors>().SendEnemyCollision(); // Take the enemies life
            enemiesHealth = new List<IHittableObject>(enemyList);

            if (snowCollider.size.x >= snowExpansionMaxSize) // If collider arrives to max size
            {
                attackFlagType = AttackFlagType.SnowAttack;
                HitEnemy(snowDamage, enemiesHealth, attackFlagType); // Send damage to enemies (TO CHANGE IN THE FUTURE, the enemies cant wait to be damage when the attack ends, have to do it while is happening)
                snowCollider.size = new Vector2(0, snowCollider.size.y);  // Return collider to normal size
                sizeSnowExpansion = 0;
                snowExpand = false;
            }
        }

        if (snowAttackTimer > 0 && !isSnowAttacking) // Cooldown only starts when the snow attack has finished
        {
            snowAttackTimer -= Time.deltaTime;
        }
        //Snow_

        if (cancelLayers)
        {
            ReceiveAnAttack(0);
        }
    }

    public IEnumerator SnowSpecialAttack() // This coroutine controls the snow attack
    {
        characterPlayerController.enabled = false; // First deactivate the character player controller of the player
        isSnowAttacking = true; // Activates the attack

        rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse); // Do a force to the ground in case the player stais on air go to the ground

        yield return new WaitForSeconds(snowPreparationTime); // Wait to the preparation time

        yield return new WaitUntil(() => isGrounded); // Wait until touching the ground

        snowGameObj = Instantiate(snowPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);

        snowExpand = true; // Start the attack collider expansion

        yield return new WaitUntil(() => !snowExpand); // Wait until the expansion arrives to the end

        activeRoutine = StartCoroutine(RecoverFromAttack()); // Start the recovery time
    }

    void HitEnemy(float damage, List<IHittableObject> hittableObjs, AttackFlagType flag) // This attacks uses other method of hitting enemies, but works the same way
    {
        if(hittableObjs.Count > 0)
        {
            Debug.Log("Enemy Hit with: " + damage);

            for (int i = 0; i < hittableObjs.Count; i++)
            {
                hittableObjs[i].ReceiveDamage(damage, flag); // For all enemies send the damage to recieve
            }

            hittableObjs.Clear(); // Clear the enemy list when the attack ends
        }
    }

    void ReceiveAnAttack(float damage) // This function executes when an enemy hits the player
    {
        if(activeRoutine != null)
        {
            StopCoroutine(activeRoutine); // Stop the coroutine that is happening in this moment

            characterPlayerController.enabled = true; // Reactivate the player

            if(snowGameObj != null)
            {
                Destroy(snowGameObj);
            }

            // Desactivate abilities
            if (isSnowAttacking)
            {
                snowCollider.size = new Vector2(0, snowCollider.size.y); // Return the original values of the snow attack, except the cooldown
                sizeSnowExpansion = 0;
                snowExpand = false;

                isSnowAttacking = false;
            }

            m_PlayerCombat.isAttacking = false;
        }
    }

    public IEnumerator RecoverFromAttack()
    {
        // Snow recovery from the attack
        if(isSnowAttacking)
        {
            yield return new WaitForSeconds(snowRecuperationTime);
            isSnowAttacking = false;
        }

        // Return player to normal state
        characterPlayerController.enabled = true;
        rb.gravityScale = defaultGravity;
        rigidbodyFreeze = false;
        m_PlayerCombat.isAttacking = false;
    }
}

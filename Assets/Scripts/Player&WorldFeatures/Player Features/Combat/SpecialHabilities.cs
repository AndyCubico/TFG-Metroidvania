using PlayerController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialHabilities : MonoBehaviour
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

    [Header("Colliders")]
    [Space(5)]
    public BoxCollider2D groundCollider;
    public BoxCollider2D snowCollider;

    [Header("Layers")]
    [Space(5)]
    public LayerMask groundLayer;

    float defaultGravity;

    bool isGrounded;
    bool isSnowAttacking;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference SpecialHabilitiesTrigger;
    public InputActionReference SnowHability;

    private List<EnemyHealth> enemiesHealth;

    bool usingController;
    bool specialHabilitiesTrigger;
    bool snowHability;

    private Coroutine activeRoutine;

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
        rigidbodyFreeze = false;
        isSnowAttacking = false;
        snowExpand = false;
        defaultGravity = rb.gravityScale;
        snowAttackTimer = 0;
        sizeSnowExpansion = 0;

        snowCollider.size = new Vector2(0, snowCollider.size.y);
    }

    void Update()
    {
        //Check if is there is something at LeftAttack
        isGrounded = Physics2D.OverlapAreaAll(groundCollider.bounds.min, groundCollider.bounds.max, groundLayer).Length > 0;

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

        if (!characterPlayerController.isDashing)
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

                if (specialHabilitiesTrigger && snowHability && !isSnowAttacking && snowAttackTimer <= 0)
                {
                    snowAttackTimer = snowAttackCooldown;

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

                if (snowHability && !isSnowAttacking && snowAttackTimer <= 0)
                {
                    snowAttackTimer = snowAttackCooldown;

                    activeRoutine = StartCoroutine(SnowSpecialAttack());
                }
            }
        }

        //_Snow
        if (snowExpand)
        {
            //Collider Expansion
            sizeSnowExpansion += Time.deltaTime * snowExpansionSpeed;
            snowCollider.size = new Vector2(sizeSnowExpansion, snowCollider.size.y);

            enemiesHealth = snowCollider.gameObject.GetComponent<Attack_Detectors>().SendEnemyCollision();

            if (snowCollider.size.x >= snowExpansionMaxSize)
            {
                HitEnemy(snowDamage, enemiesHealth);
                snowCollider.size = new Vector2(0, snowCollider.size.y);
                sizeSnowExpansion = 0;
                snowExpand = false;
            }
        }

        if (snowAttackTimer > 0 && !isSnowAttacking)
        {
            Debug.Log("Snow Cooldown");
            snowAttackTimer -= Time.deltaTime;
        }
        //Snow_
    }

    public IEnumerator SnowSpecialAttack()
    {
        Debug.Log("Snow Attack Preparation");
        characterPlayerController.enabled = false;
        isSnowAttacking = true;

        yield return new WaitForSeconds(snowPreparationTime);

        rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse);

        yield return new WaitUntil(() => isGrounded);

        snowExpand = true;
        Debug.Log("Snow Attack");

        yield return new WaitUntil(() => !snowExpand);

        activeRoutine = StartCoroutine(RecuperateFromAttack());
    }

    void HitEnemy(float damage, List<EnemyHealth> enemyHealth)
    {
        if(enemyHealth.Count > 0)
        {
            Debug.Log("Enemy Hit with: " + damage);

            for (int i = 0; i < enemyHealth.Count; i++)
            {
                enemyHealth[i].ReceiveDamage(damage);
            }

            enemyHealth.Clear();
        }
    }

    void ReceiveAnAttack()
    {
        if(activeRoutine != null)
        {
            StopCoroutine(activeRoutine);

            characterPlayerController.enabled = true;

            // Desactivate abilities
            isSnowAttacking = false;
        }
    }

    public IEnumerator RecuperateFromAttack()
    {
        // Snow recovery from the attack
        if(isSnowAttacking)
        {
            Debug.Log("Snow recuperate");
            yield return new WaitForSeconds(snowRecuperationTime);
            isSnowAttacking = false;
        }

        characterPlayerController.enabled = true;
        rb.gravityScale = defaultGravity;
        rigidbodyFreeze = false;
    }
}

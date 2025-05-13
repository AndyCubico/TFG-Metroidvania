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
    public float snowAttackDuration;
    private float snowAttackTimer;
    public float downForce;
    public float snowDamage;
    public float snowExpansionSpeed;
    public float snowExpansion;
    private bool snowExapnd;
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

    private void OnEnable()
    {
        SpecialHabilitiesTrigger.action.started += SpecialHabilitiesEvent;
        SnowHability.action.started += SnowHabilityEvent;
    }

    private void OnDisable()
    {
        SpecialHabilitiesTrigger.action.started -= SpecialHabilitiesEvent;
        SnowHability.action.started -= SnowHabilityEvent;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbodyFreeze = false;
        isSnowAttacking = false;
        snowExapnd = false;
        defaultGravity = rb.gravityScale;
        snowAttackTimer = 0;
        sizeSnowExpansion = 0;

        snowCollider.size = new Vector2(0, snowCollider.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if is there is something at LeftAttack
        isGrounded = Physics2D.OverlapAreaAll(groundCollider.bounds.min, groundCollider.bounds.max, groundLayer).Length > 0;

        if (Gamepad.current != null)
        {
            usingController = true;
        }
        // Verifica si hay un teclado conectado
        else if (Keyboard.current != null)
        {
            usingController = false;
        }
        // Si no hay ni teclado ni mando conectado, muestra un mensaje de advertencia
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
                    snowAttackTimer = snowAttackDuration;

                    SnowSpecialAttack();
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
                    snowAttackTimer = snowAttackDuration;

                    StartCoroutine(SnowSpecialAttack());
                }
            }
        }

        //_Snow
        if (snowExapnd)
        {
            //Collider Expansion
            sizeSnowExpansion += Time.deltaTime * snowExpansionSpeed;
            snowCollider.size = new Vector2(sizeSnowExpansion, snowCollider.size.y);

            enemiesHealth = snowCollider.gameObject.GetComponent<Attack_Detectors>().SendEnemyCollision();

            if (snowCollider.size.x >= snowExpansion)
            {
                HitEnemy(snowDamage, enemiesHealth);
                snowCollider.size = new Vector2(0, snowCollider.size.y);
                sizeSnowExpansion = 0;
                snowExapnd = false;
            }
        }

        if (snowAttackTimer > 0 && !isSnowAttacking)
        {
            snowAttackTimer -= Time.deltaTime;
        }
        //Snow_

        if (!isSnowAttacking /*Other abilities*/)
        {
            characterPlayerController.enabled = true;
            rb.gravityScale = defaultGravity;
            rigidbodyFreeze = false;
        }
    }

    public IEnumerator SnowSpecialAttack()
    {
        characterPlayerController.enabled = false;
        isSnowAttacking = true;

        rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse);

        yield return new WaitUntil(() => isGrounded);

        snowExapnd = true;

        yield return new WaitUntil(() => !snowExapnd);

        isSnowAttacking = false;
    }

    void HitEnemy(float damage, List<EnemyHealth> enemyHealth)
    {
        Debug.Log("Enemy Hit with: " + damage);

        for (int i = 0; i < enemyHealth.Count; i++)
        {
            enemyHealth[i].ReceiveDamage(damage);
        }

        enemyHealth.Clear();
    }
}

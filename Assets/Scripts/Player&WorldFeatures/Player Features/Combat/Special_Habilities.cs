using PlayerController;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Special_Habilities : MonoBehaviour
{
    [Header("Character Player Controller")]
    [Space(5)]
    public CharacterPlayerController characterPlayerController;

    [Header("Rigidbody")]
    [Space(5)]
    public Rigidbody2D rb;
    bool rigidbodyFreeze;

    [Header("Habilities Time")]
    [Space(5)]
    public float snowAttackDuration;
    private float snowAttackTimer;

    float defaultGravity;

    bool isSnowAttacking;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference SpecialHabilitiesTrigger;
    public InputActionReference SnowHability;

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
        defaultGravity = rb.gravityScale;
        snowAttackTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (snowAttackTimer > 0 && !isSnowAttacking)
        {
            snowAttackTimer -= Time.deltaTime;
        }

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

        //while (!characterPlayerController.isGrounded)
        //{

        //}

        Debug.Log("Snow Attack!");

        yield return new WaitForSeconds(2f);
        
        isSnowAttacking = false;
        Debug.Log("Snow Attack Finished!");
    }
}

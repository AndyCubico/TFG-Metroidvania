using PlayerController;
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

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference SpecialHabilitiesTrigger;
    public InputActionReference SnowHability;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(!characterPlayerController.isDashing)
        {
            characterPlayerController.enabled = !specialHabilitiesTrigger;

            if (specialHabilitiesTrigger && !rigidbodyFreeze)
            {
                rb.linearVelocity = Vector3.zero;
                rigidbodyFreeze = true;
            }
            else if (!specialHabilitiesTrigger)
            {
                rigidbodyFreeze = false;
            }

            if (specialHabilitiesTrigger && snowHability)
            {
                SnowSpecialAttack();
            }
        }
    }

    void SnowSpecialAttack()
    {
        Debug.Log("Snow Attack!");
    }
}

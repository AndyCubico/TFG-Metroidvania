using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockAbilities : MonoBehaviour
{
    private GameObject m_PressEObj;

    [Header("Input Actions")]
    public InputActionReference InteractionKeyAction;
    bool InteractionInput;
    bool hasBeenUnlocked;

    public SpecialAbility specialAbility;

    void Start()
    {
        m_PressEObj = this.gameObject.transform.GetChild(0).gameObject;
        hasBeenUnlocked = false;
    }

    void Update()
    {
        if (InteractionKeyAction.action.WasPressedThisFrame())
        {
            InteractionInput = true;
        }
        else
        {
            InteractionInput = false;
        }
    }

    [System.Obsolete]
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!hasBeenUnlocked)
        {
            if (collision.CompareTag("Player") && collision.gameObject.layer != 12)
            {
                if (m_PressEObj != null)
                {
                    if (!m_PressEObj.active)
                    {
                        m_PressEObj.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogWarning("Put the m_PressEObj in the Checkpoint_Station script");
                }

                SpecialAbilities specialAbilities = GameObject.Find("SpecialAttacks").GetComponent<SpecialAbilities>();
                PlayerCombatV2 playerCombat = collision.transform.FindChild("Combat").GetComponent<PlayerCombatV2>();

                if (playerCombat != null && specialAbilities != null)
                {
                    playerCombat.isInCheckpoint = true;

                    if (InteractionInput)
                    {
                        playerCombat.isInCheckpoint = false;

                        if (m_PressEObj != null)
                        {
                            if (m_PressEObj.active)
                            {
                                m_PressEObj.SetActive(false);
                            }
                        }

                        switch (specialAbility)
                        {
                            case SpecialAbility.SNOW:
                                specialAbilities.snowAbilityUnlocked = true;
                                break;
                            case SpecialAbility.SUN:
                                break;
                        }

                        specialAbilities.snowAbilityUnlocked = true;
                        hasBeenUnlocked = true;

                        Debug.Log("Snow ability unlocked!");
                    }
                }
            }
        }
    }
}

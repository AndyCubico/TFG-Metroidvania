using UnityEngine;
using UnityEngine.InputSystem;

public class UnlockAbilities : MonoBehaviour
{
    private GameObject m_PressEObj;

    [Header("Input Actions")]
    public InputActionReference InteractionKeyAction;
    bool InteractionInput;
    bool hasBeenUnlocked;
    bool m_IsInside;

    public SpecialAbility specialAbility;

    void Start()
    {
        m_PressEObj = this.gameObject.transform.GetChild(0).gameObject;
        hasBeenUnlocked = false;
        m_IsInside = false;
    }

    void Update()
    {
        if (InteractionKeyAction.action.WasPressedThisFrame() && m_IsInside)
        {
            InteractionInput = true;
        }
    }

    [System.Obsolete]
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!hasBeenUnlocked)
        {
            if (collision.CompareTag("Player") && collision.gameObject.layer != 12)
            {
                m_IsInside = true;

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
                PlayerCombatV2 playerCombat = collision.transform.Find("Combat").GetComponent<PlayerCombatV2>();
                PlayerBlockAndParry playerBlockParry = collision.transform.Find("Combat").Find("Block_Parry").GetComponent<PlayerBlockAndParry>();

                if (playerCombat != null && specialAbilities != null && playerBlockParry != null)
                {
                    playerCombat.isInCheckpoint = true;
                    playerBlockParry.isInCheckpoint = true;

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
                        InteractionInput = false;
                        this.gameObject.SetActive(false);

                        Debug.Log("Snow ability unlocked!");
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasBeenUnlocked)
        {
            if (collision.CompareTag("Player") && collision.gameObject.layer != 12)
            {
                m_IsInside = false;

                if (m_PressEObj != null)
                {
                    if (m_PressEObj.active)
                    {
                        m_PressEObj.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogWarning("Put the m_PressEObj in the Checkpoint_Station script");
                }

                PlayerBlockAndParry playerBlockParry = collision.transform.Find("Combat").Find("Block_Parry").GetComponent<PlayerBlockAndParry>();

                if (playerBlockParry != null)
                {
                    playerBlockParry.isInCheckpoint = false;
                }
            }
        }
    }
}

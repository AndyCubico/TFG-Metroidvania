using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint_Station : MonoBehaviour
{   
    private GameObject m_PressEObj;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference InteractionKeyAction;
    bool InteractionInput;

    bool m_IsInside;

    void Start()
    {
        m_PressEObj = this.gameObject.transform.GetChild(0).gameObject;
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

            PlayerCombatV2 playerCombat = collision.transform.FindChild("Combat").GetComponent<PlayerCombatV2>();
            PlayerBlockAndParry playerBlockParry = collision.transform.FindChild("Combat").FindChild("Block_Parry").GetComponent<PlayerBlockAndParry>();

            if (playerCombat != null)
            {
                playerCombat.isInCheckpoint = true;
                playerBlockParry.isInCheckpoint = true;

                if (InteractionInput)
                {
                    HealthEvents.eRestoreHealth?.Invoke();
                    HealthEvents.eRestorePotions?.Invoke();
                    SaveAndLoadEvents.eSaveAction?.Invoke();
                    InteractionInput = false;
                    Debug.Log("Saved!");
                }
            }
        }
    }

    [System.Obsolete]
    private void OnTriggerExit2D(Collider2D collision)
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

            PlayerCombatV2 playerCombat = collision.transform.FindChild("Combat").GetComponent<PlayerCombatV2>();
            PlayerBlockAndParry playerBlockParry = collision.transform.FindChild("Combat").FindChild("Block_Parry").GetComponent<PlayerBlockAndParry>();

            if (playerCombat != null)
            {
                playerCombat.isInCheckpoint = false;
                playerBlockParry.isInCheckpoint = false;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint_Station : MonoBehaviour
{   
    private GameObject m_PressEObj;

    [Header("Input Actions")]
    public InputActionReference InteractionKeyAction;
    bool InteractionInput;

    void Start()
    {
        m_PressEObj = this.gameObject.transform.GetChild(0).gameObject;
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

            PlayerCombatV2 playerCombat = collision.transform.FindChild("Combat").GetComponent<PlayerCombatV2>();

            if (playerCombat != null)
            {
                playerCombat.isInCheckpoint = true;

                if(InteractionInput)
                {
                    SaveAndLoadEvents.eSaveAction?.Invoke();
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

            if (playerCombat != null)
            {
                playerCombat.isInCheckpoint = false;
            }
        }
    }
}

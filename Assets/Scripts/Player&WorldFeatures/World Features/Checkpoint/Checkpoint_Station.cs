using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint_Station : MonoBehaviour
{   
    private GameObject m_PressEObj;
    private TextMeshProUGUI m_SavedUIText;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference InteractionKeyAction;
    bool InteractionInput;

    bool m_IsInside;
    bool m_hasSaved;
    private float speedDissapearText = 2f;

    void Start()
    {
        m_PressEObj = gameObject.transform.GetChild(0).gameObject;
        m_SavedUIText = GameObject.Find("Saved!").GetComponent<TextMeshProUGUI>();
        m_IsInside = false;
        m_hasSaved = false;
    }

    void Update()
    {
        if (InteractionKeyAction.action.WasPressedThisFrame() && m_IsInside)
        {
            InteractionInput = true;
        }

        if (m_SavedUIText.color.a > -0.1 && m_hasSaved)
        {
            m_SavedUIText.color = new Color(m_SavedUIText.color.r, m_SavedUIText.color.g, m_SavedUIText.color.b, m_SavedUIText.color.a - Time.deltaTime * speedDissapearText);

            if (m_SavedUIText.color.a <= 0)
            {
                m_hasSaved = false;
            }
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

                    m_SavedUIText.color = new Color(m_SavedUIText.color.r, m_SavedUIText.color.g, m_SavedUIText.color.b, 1);
                    InteractionInput = false;
                    m_hasSaved = true;
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

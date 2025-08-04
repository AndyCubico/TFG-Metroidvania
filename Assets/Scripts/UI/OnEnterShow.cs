using System.Collections;
using System.Linq;
using UnityEngine;

public class OnEnterShow : MonoBehaviour
{
    [SerializeField] GameObject m_ObjectToShow;
    [SerializeField] bool m_IsHide;
    [SerializeField] bool m_IsShowOnce;
    [SerializeField] bool m_IsHideOnce;
    bool m_Show = true; // External condition to permanently hide the object if false
    [TagDropdown] public string[] collisionTag = new string[] { };

    public float timeForShow = 0.0f; // Time the gameObject must stay inside to make hidden object visible.
    private GameObject triggeringObject = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_Show && collisionTag.Contains(collision.gameObject.tag) && triggeringObject == null || triggeringObject == collision.gameObject)
        {
            triggeringObject = collision.gameObject;
            if (timeForShow > 0)
            {
                timeForShow -= Time.deltaTime;
            }
            else
            {
                m_ObjectToShow.SetActive(!m_IsHide);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggeringObject == collision.gameObject) // Only when the og go moves out, it stops showwing
        {
            if (!m_IsHideOnce)
            {
                m_ObjectToShow.SetActive(m_IsHide);
            }
            triggeringObject = null;
            if(m_IsShowOnce) { m_Show = false; }
        }
    }
}

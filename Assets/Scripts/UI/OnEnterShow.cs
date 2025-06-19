using System.Collections;
using System.Linq;
using UnityEngine;

public class OnEnterShow : MonoBehaviour
{
    [SerializeField] GameObject m_ObjectToShow;
    [SerializeField] bool isHide;
    [TagDropdown] public string[] collisionTag = new string[] { };

    public float timeForShow = 0.0f; // Time the gameObject must stay inside to make hidden object visible.
    private GameObject triggeringObject = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && triggeringObject == null || triggeringObject == collision.gameObject)
        {
            triggeringObject = collision.gameObject;
            if (timeForShow > 0)
            {
                timeForShow -= Time.deltaTime;
                print("WaitTime: " + timeForShow);

            }
            else
            {
                m_ObjectToShow.SetActive(!isHide);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggeringObject == collision.gameObject) // Only when the og go moves out, it stops showwing
        {
            m_ObjectToShow.SetActive(isHide);
            triggeringObject = null;
        }
    }
}

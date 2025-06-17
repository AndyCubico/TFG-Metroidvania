using System.Collections;
using System.Linq;
using UnityEngine;

public class OnEnterShow : MonoBehaviour
{
    [SerializeField] GameObject m_ObjectToShow;
    [TagDropdown] public string[] collisionTag = new string[] { };

    public float timeForShow = 0.0f; // Time the gameObject must stay inside to make hidden object visible.
    private GameObject tiggeingObject = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && tiggeingObject == null || tiggeingObject == collision.gameObject)
        {
            tiggeingObject = collision.gameObject;
            if (timeForShow > 0)
            {
                timeForShow -= Time.deltaTime;
                print("WaitTime: " + timeForShow);

            }
            else
            {
                m_ObjectToShow.SetActive(true);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tiggeingObject == collision.gameObject) // Only when the og go moves out, it stops showwing
        {
            m_ObjectToShow.SetActive(false);
            tiggeingObject = null;
        }
    }
}

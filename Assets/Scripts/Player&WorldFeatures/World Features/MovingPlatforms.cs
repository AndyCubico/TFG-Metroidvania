using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MovingPlatforms : MonoBehaviour
{
    //[SerializeField] Dictionary<GameObject, Transform> m_ObjectOriginalParent;
    [SerializeField] List<Transform> m_Objects;
    [TagDropdown] public string[] collisionTag = new string[] { };
    [SerializeField] LayerMask m_ExcludedLayerMask;
    Vector3 m_LastPosition;
    Vector3 m_Displacement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_ObjectOriginalParent = new Dictionary<GameObject, Transform> { };
        m_LastPosition = transform.position;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collisionTag.Contains(collision.gameObject.tag) && !m_ObjectOriginalParent.ContainsKey(collision.gameObject))
    //    {
    //        m_ObjectOriginalParent.Add(collision.gameObject, collision.transform.parent);
    //        collision.gameObject.transform.parent = this.transform;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && !m_Objects.Contains(collision.transform) && (m_ExcludedLayerMask & (1 << collision.gameObject.layer)) == 0) // First check
        {
            bool childAlready = false;
            for (int i = 0; i < m_Objects.Count; i++) 
            {
                childAlready = collision.transform.IsChildOf(m_Objects[i]);
                if (childAlready) 
                {
                    return; //If already has child not add transform to the list
                }
            }
            m_Objects.Add(collision.transform);
        }
    }

    private void LateUpdate()
    {
        m_Displacement = (transform.position - m_LastPosition) / Time.deltaTime;
        m_LastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < m_Objects.Count; i++)
        {
            m_Objects[i].position += m_Displacement * Time.fixedDeltaTime;
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (m_ObjectOriginalParent.ContainsKey(collision.gameObject))
    //    {
    //        Transform t;
    //        m_ObjectOriginalParent.TryGetValue(collision.gameObject, out t);
    //        collision.gameObject.transform.parent = t;
    //        m_ObjectOriginalParent.Remove(collision.gameObject);
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_Objects.Contains(collision.transform))
        {
            m_Objects.Remove(collision.transform);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] Dictionary<GameObject, Transform> m_ObjectOriginalParent;
    [TagDropdown] public string[] collisionTag = new string[] { };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_ObjectOriginalParent = new Dictionary<GameObject, Transform> { };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && !m_ObjectOriginalParent.ContainsKey(collision.gameObject)) 
        {
            m_ObjectOriginalParent.Add(collision.gameObject,collision.transform.parent);
            collision.gameObject.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_ObjectOriginalParent.ContainsKey(collision.gameObject))
        {
            Transform t;
            m_ObjectOriginalParent.TryGetValue(collision.gameObject, out t);
            collision.gameObject.transform.parent = t;
            m_ObjectOriginalParent.Remove(collision.gameObject);
        }
    }
}

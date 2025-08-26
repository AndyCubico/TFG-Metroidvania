using System.Collections;
using System.Linq;
using UnityEngine;

public class DestroyObjectPostCollision : MonoBehaviour
{
    //[TagDropdown] public string TagFilter = "";
    [SerializeField] private GameObject m_objectToDestroy;
    [TagDropdown] public string[] collisionTag = new string[] { };
    

    [SerializeField] private bool m_isRespawnOverTime;

    [Header("Timing")]
    [ShowIf("m_isRespawnOverTime",true)] public float timeToRespawn;
    public float timeToDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        if (collisionTag.Contains(collision.gameObject.tag)) 
        {
            Debug.Log("Collision with: " + collision.gameObject.tag);
            // Collision detected with corresponding tag
            StartCoroutine(DeactivateGameObject(m_objectToDestroy));
            
        }
    }

    private IEnumerator ActivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToRespawn);

        // Deactivate GameObject
        go.SetActive(true);
    }

    private IEnumerator DeactivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToDestroy);

        // Deactivate GameObject
        go.SetActive(false);

        if (m_isRespawnOverTime)
        {
            StartCoroutine(ActivateGameObject(m_objectToDestroy));
        }
    }
    

}

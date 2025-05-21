using System.Linq;
using UnityEngine;

public class DestroyObjectPostCollision : MonoBehaviour
{
    //[TagDropdown] public string TagFilter = "";

    [TagDropdown] public string[] collisionTag = new string[] { };

    [SerializeField] private bool m_isRespawnOverTime;

    [ShowIf("m_isRespawnOverTime")] public float timeToRespawn;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag)) 
        {
            // Collision detected with corresponding tag
        }
    }
}

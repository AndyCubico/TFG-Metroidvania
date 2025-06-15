using System.Linq;
using UnityEditor;
using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [TagDropdown] public string[] collisionTag = new string[] { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag))
        {
            this.gameObject.GetComponent<Rigidbody2D>().WakeUp();
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }
    }
}

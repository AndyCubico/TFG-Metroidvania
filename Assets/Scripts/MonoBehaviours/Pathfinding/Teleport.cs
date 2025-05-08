using UnityEngine;

public class Teleport : MonoBehaviour
{
    // Script for debug purposes.
    [SerializeField] private GameObject m_Enemy;
    [SerializeField] private Transform m_TP;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_Enemy.transform.position = m_TP.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.transform.position = m_TP.position; 
        }
    }
}

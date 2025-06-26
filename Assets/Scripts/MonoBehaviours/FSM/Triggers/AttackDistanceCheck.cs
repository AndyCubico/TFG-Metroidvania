using UnityEngine;

public class AttackDistanceCheck : MonoBehaviour
{
    private Enemy m_Enemy;

    private void Awake()
    {
        m_Enemy = GetComponent<Enemy>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_Enemy.SetWithinAttackRange(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_Enemy.SetWithinAttackRange(false);
        }
    }
}

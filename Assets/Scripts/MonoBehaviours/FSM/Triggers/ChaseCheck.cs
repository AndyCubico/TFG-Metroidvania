using UnityEngine;

public class ChaseCheck : MonoBehaviour
{
    private Enemy m_Enemy;

    private void Awake()
    {
        m_Enemy = GetComponent<Enemy>();
    }

    // TODO: Add vision to the enemy, so it starts chasing when it sees the player.
    // Probably raycast or something similar.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_Enemy.SetAggro(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_Enemy.SetAggro(false);
        }
    }
}

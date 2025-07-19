using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private float m_RaycastDistance;
    [SerializeField] private float m_PushForce;
    [SerializeField] private float m_PushCooldown;

    private Collider2D m_Collider;
    private Enemy m_EnemyCS;
    private float m_PushCooldownTimer;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
        m_EnemyCS = GetComponent<Enemy>();

        m_PushCooldownTimer = 0f;

        if (m_EnemyCS == null)
            Debug.LogError("PlayerGroundCheck: Enemy component not found.");
    }

    private void FixedUpdate()
    {
        if (m_PushCooldownTimer > 0f)
        {
            m_PushCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            PushPlayerIfUnderneath();
        }
    }

    private void PushPlayerIfUnderneath()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - m_Collider.bounds.extents.y - 0.01f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, m_RaycastDistance);
        Debug.DrawRay(origin, Vector2.down * m_RaycastDistance, Color.red);

        if (hit.collider != null && hit.collider.transform.root.CompareTag("Player"))
        {
            Rigidbody2D playerRb = hit.collider.attachedRigidbody;

            if (playerRb != null && m_EnemyCS != null)
            {
                // Push the player in the opposite direction the enemy is facing
                float pushDirection = m_EnemyCS.isFacingRight ? -1f : 1f;
                Vector2 force = new Vector2(pushDirection * m_PushForce, 0f);
                playerRb.AddForce(force, ForceMode2D.Force);

                m_PushCooldownTimer = m_PushCooldown;
            }
        }
    }
}

using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private float pushForce = 200f;

    private Collider2D col;
    private Enemy enemy;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        enemy = GetComponent<Enemy>();

        if (enemy == null)
            Debug.LogError("PlayerGroundCheck: Enemy component not found on this GameObject.");
    }

    private void FixedUpdate()
    {
        PushPlayerIfUnderneath();
    }

    private void PushPlayerIfUnderneath()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - col.bounds.extents.y - 0.01f);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance);
        Debug.DrawRay(origin, Vector2.down * raycastDistance, Color.red);

        if (hit.collider != null && hit.collider.transform.root.CompareTag("Player"))
        {
            Rigidbody2D playerRb = hit.collider.attachedRigidbody;

            if (playerRb != null && enemy != null)
            {
                // Push the player in the opposite direction the enemy is facing
                float pushDirection = enemy.isFacingRight ? -1f : 1f;
                Vector2 force = new Vector2(pushDirection * pushForce, 0f);
                playerRb.AddForce(force, ForceMode2D.Force);
            }
        }
    }
}

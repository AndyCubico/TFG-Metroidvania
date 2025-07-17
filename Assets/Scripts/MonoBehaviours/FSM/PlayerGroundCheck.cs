using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private float pushForce = 200f;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void FixedUpdate()
    {
        if (IsStandingOnPlayer())
        {
            PushAwayFromPlayer();
        }
    }

    private bool IsStandingOnPlayer()
    {
        // Ray origin from the bottom of the enemy
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - col.bounds.extents.y - 0.01f);

        // Raycast downward using only the player layer
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, playerLayer);
        Debug.DrawRay(origin, Vector2.down * raycastDistance, Color.green);

        // Check if the hit is tagged as Player
        return hit.collider != null && hit.collider.transform.root.CompareTag("Player");
    }

    private void PushAwayFromPlayer()
    {
        if (playerTransform == null) return;

        // Push away horizontally from player position
        float direction = transform.position.x < playerTransform.position.x ? -1f : 1f;
        rb.AddForce(new Vector2(direction * pushForce, 0f), ForceMode2D.Force);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // prevent horizontal push
        }
    }
}

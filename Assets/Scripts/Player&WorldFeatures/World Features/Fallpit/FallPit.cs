using PlayerController;
using System.Collections;
using UnityEngine;

public class FallPit : MonoBehaviour
{
    public float timeToCheckpoint;
    private PlayerHealth health;

    private float damage = 20f;
    //public float upForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject.layer != 12) //No Hang Edges
        {
            //collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
            collision.GetComponent<CharacterPlayerController>().enabled = false; 

            health = collision.GetComponent<PlayerHealth>();

            if (health.playerHealth - damage > 0)
            {
                StartCoroutine(TpFastCheckpoint());
            }

            HealthEvents.TakingDamage(damage);
        }
    }

    public IEnumerator TpFastCheckpoint()
    {
        yield return new WaitForSeconds(timeToCheckpoint);
        
        CheckpointEvents.FastCheckpointEvent?.Invoke();
    }
}

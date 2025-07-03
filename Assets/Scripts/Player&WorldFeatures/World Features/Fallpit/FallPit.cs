using PlayerController;
using System.Collections;
using UnityEngine;

public class FallPit : MonoBehaviour
{
    public float timeToCheckpoint;
    //public float upForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
            collision.GetComponent<CharacterPlayerController>().enabled = false; HealthEvents.TakingDamage(20);
            StartCoroutine(TpFastCheckpoint());
        }
    }

    public IEnumerator TpFastCheckpoint()
    {
        yield return new WaitForSeconds(timeToCheckpoint);

        CheckpointEvents.FastCheckpointEvent?.Invoke();
    }
}

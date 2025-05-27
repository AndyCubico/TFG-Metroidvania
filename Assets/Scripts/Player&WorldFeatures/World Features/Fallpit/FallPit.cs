using UnityEngine;

public class FallPit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            CheckpointEvents.FastCheckpointEvent?.Invoke();
        }
    }
}

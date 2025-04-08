using UnityEngine;
using UnityEngine.Rendering;

public class HangEdges : MonoBehaviour
{
    public Transform playerPosition;
    private CharacterPlayerController playerController;
    private bool isHanged;
    public BoxCollider2D ground;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<CharacterPlayerController>();

            playerController.isHangingEdge = true;

            collision.gameObject.transform.position = playerPosition.position;
            collision.gameObject.transform.rotation = playerPosition.rotation;

            ground.isTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ground.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ground.isTrigger = false;
        }
    }
}

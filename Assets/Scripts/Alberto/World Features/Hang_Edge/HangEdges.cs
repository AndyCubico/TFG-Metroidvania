using UnityEngine;
using UnityEngine.Rendering;

public class HangEdges : MonoBehaviour
{
    public Transform playerPosition;
    private GameObject player;
    private CharacterPlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Edge"))
        {
            playerController = collision.transform.parent.parent.GetComponent<CharacterPlayerController>();

            playerController.isHangingEdge = true;
            player = collision.transform.parent.parent.gameObject;
            player.transform.position = playerPosition.transform.position;
        }
    }
}

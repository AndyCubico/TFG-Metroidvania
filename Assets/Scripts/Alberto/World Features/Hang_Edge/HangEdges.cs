using UnityEngine;
using UnityEngine.Rendering;

public class HangEdges : MonoBehaviour
{
    public float transaltionSpeed;
    public Transform playerPosition;
    private GameObject player;
    private CharacterPlayerController playerController;
    private bool isHanged;

    private void Update()
    {
        if (isHanged)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, playerPosition.transform.position, transaltionSpeed);

            if(player.transform.position == playerPosition.transform.position)
            {
                isHanged = false;
            }
        }

        if(playerController != null)
        {
            if (playerController.jumpKeyHold)
            {
                isHanged = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<CharacterPlayerController>();

            playerController.isHangingEdge = true;
            player = collision.gameObject;

            //collision.gameObject.transform.position = playerPosition.position;
            //collision.gameObject.transform.rotation = playerPosition.rotation;

            isHanged = true;
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;

public class HangEdges : MonoBehaviour
{
    public Transform playerPosition;
    public Transform playerFinalPosition;
    private GameObject player;
    private CharacterPlayerController playerController;
    public bool isHanged;
    public bool moveToNewPosition;
    public float speedTransform;

    private void Start()
    {
        isHanged = false;
    }

    private void Update()
    {
        if (isHanged)
        {
            if(playerController.canUnhang && playerController.jumpKeyHold)
            {
                moveToNewPosition = true;
                isHanged= false;
            }
        }

        if (moveToNewPosition)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerFinalPosition.position, speedTransform * Time.deltaTime);

            playerController.jumpStopper = true;

            if (Vector3.Distance(player.transform.position, playerFinalPosition.position) < 0.1f || Vector3.Distance(player.transform.position, playerFinalPosition.position) > 5f)
            {
                playerController.rb.linearVelocity = Vector2.zero;

                if(!playerController.jumpKeyHold)
                {
                    playerController.jumpStopper = false;
                    playerController.playerOnEdgeUnfrezze = true;
                    moveToNewPosition = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Edge"))
        {
            playerController = collision.transform.parent.parent.GetComponent<CharacterPlayerController>();

            playerController.isHangingEdge = true;
            player = collision.transform.parent.parent.gameObject;
            player.transform.position = playerPosition.transform.position;
            isHanged = true;
        }
    }
}

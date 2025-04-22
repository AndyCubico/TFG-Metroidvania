using UnityEngine;
using UnityEngine.Rendering;
using Player_Controller;

public class Hang_Edges : MonoBehaviour
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
        if (isHanged) //Once the player is hanged
        {
            if(playerController.canUnhang && playerController.climbEdges) //If the player has released the space button and the climb edge key is pressed move the player to the new position
            {
                moveToNewPosition = true;
                isHanged= false;
            }
        }

        if (moveToNewPosition) //Here is where the player will be moved to the exit position
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, playerFinalPosition.position, speedTransform * Time.deltaTime); //The function that moves the player

            playerController.jumpStopper = true;

            if (Vector3.Distance(player.transform.position, playerFinalPosition.position) < 0.1f || Vector3.Distance(player.transform.position, playerFinalPosition.position) > 5f) //If the player is close enought or to far away exit the hang situation
            {
                playerController.rb.linearVelocity = Vector2.zero;

                if(!playerController.jumpKeyHold) //Check that the player is not pressing the jump button to not make an involuntary jump when ending the climb
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

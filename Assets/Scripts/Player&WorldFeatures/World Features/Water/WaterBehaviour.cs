using PlayerController;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    bool isFreezed = false;
    bool playerIsInsde = false;
    CharacterPlayerController playerController;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<CharacterPlayerController>();
    }

    public void FreezeWater()
    {
        isFreezed = true;
        playerController.isInWater = false;

        if (playerIsInsde && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.WALL && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.HANGED)
        {
            playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y + (this.transform.localScale.y / 2), playerController.transform.position.z);

            playerIsInsde = false;
        }

        this.GetComponent<BoxCollider2D>().isTrigger = false;
        gameObject.layer = 7;
    }

    public void UnFreezeWater()
    {
        isFreezed = false;
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.layer = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFreezed && !playerController.isInWater && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.WALL && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.HANGED)
        {
            playerController.isInWater = true;
            playerIsInsde = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFreezed)
        {
            playerController.isInWater = false;
            playerIsInsde = false;
        }
    }
}

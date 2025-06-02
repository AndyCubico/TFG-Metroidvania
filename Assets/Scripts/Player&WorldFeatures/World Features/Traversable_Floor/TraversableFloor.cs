using PlayerController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraversableFloor : MonoBehaviour
{
    private GameObject player;
    private CharacterPlayerController characterPlayerController;
    private bool isDropping;
    private bool isOnTop;

    private BoxCollider2D ground;

    private bool isOnPlatform;

    private float breakDistanceConexion;

    void Start()
    {
        ground = this.transform.GetChild(0).GetComponent<BoxCollider2D>();
        breakDistanceConexion = this.transform.localScale.x;

        isDropping = false;
        isOnTop = false;
    }

    void Update()
    {
        if (isOnPlatform)
        {
            float distance = 0;
            distance = transform.position.y - (player.transform.position.y - (player.transform.localScale.y / 2) + 0.1f);

            if (distance < 0f)
            {
                isOnTop = true;
            }
            else if (distance >= 0f)
            {
                isOnTop = false;
                isDropping = false;
            }

            if(Mathf.Abs(distance) >= breakDistanceConexion)
            {
                isOnPlatform = false;
            }

            if(isOnTop && ground.isTrigger && !isDropping)
            {
                ground.isTrigger = false;
            }
            else if(!isOnTop && !ground.isTrigger)
            {
                ground.isTrigger = true;
            }

            if((characterPlayerController.downControllerSensitivity < -0.8f || characterPlayerController.dropDown) && !isDropping)
            {
                ground.isTrigger = true;
                isDropping = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;

            if(characterPlayerController == null)
            {
                characterPlayerController = player.GetComponent<CharacterPlayerController>();
            }

            breakDistanceConexion = player.transform.localScale.y + 0.2f;
            isOnPlatform = true;
        }
    }
}

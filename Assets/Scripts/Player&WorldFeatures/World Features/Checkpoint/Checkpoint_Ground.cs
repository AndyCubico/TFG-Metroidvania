using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using Unity.Physics;

public class Checkpoint_Ground : MonoBehaviour
{
    public GameObject checkpoint;
    CharacterPlayerController characterController;
    Rigidbody2D rb;

    BoxCollider2D boxCollider;
    CircleCollider2D circleCollider;

    public float timeToMoveNextPosition;
    float counterToMoveNextPosition;

    public float speedTranistion;
    bool isTransitioning;

    float gravityScale;

    public LayerMask groundMask;

    Transform lastSecurePosition;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterPlayerController>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        gravityScale = rb.gravityScale;

        checkpoint.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - (this.transform.localScale.y / 2), this.gameObject.transform.position.z);

        lastSecurePosition = checkpoint.transform;

        isTransitioning = false;
        counterToMoveNextPosition = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            counterToMoveNextPosition += Time.deltaTime;

            if (counterToMoveNextPosition >= timeToMoveNextPosition)
            {
                if (characterController.isGrounded)
                {
                    RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, Vector2.down, 1f, groundMask);

                    if (hit.collider != null)
                    {
                        lastSecurePosition = checkpoint.transform;

                        checkpoint.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - (this.transform.localScale.y / 2), this.gameObject.transform.position.z);
                    }
                    else
                    {
                        checkpoint.transform.position = lastSecurePosition.transform.position;
                    }

                    counterToMoveNextPosition = 0;
                }
            }
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y + this.transform.localScale.y / 2, checkpoint.transform.position.z), speedTranistion);

            if(this.transform.position == new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y + this.transform.localScale.y / 2, checkpoint.transform.position.z))
            {
                characterController.enabled = true;
                rb.gravityScale = gravityScale;

                boxCollider.isTrigger = false;
                circleCollider.isTrigger = false;

                isTransitioning = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            FastCheckpoint();
        }
    }

    public void FastCheckpoint()
    {
        characterController.enabled = false;
        rb.gravityScale = 0f;

        boxCollider.isTrigger = true;
        circleCollider.isTrigger = true;

        isTransitioning = true;
    }
}

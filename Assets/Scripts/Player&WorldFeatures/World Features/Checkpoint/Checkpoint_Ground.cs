using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using Unity.Physics;
using System;

public static class CheckpointEvents
{
    public static Action FastCheckpointEvent;
}

public class Checkpoint_Ground : MonoBehaviour
{
    [Header("______________________CHECKPOINT______________________")]
    [Space(5)]
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

    [Space(5)]

    [Header("______________________FADE TO BLACK______________________")]
    [Space(5)]
    public float FadeIn;
    public float FadeOut;

    private float m_SecondsAfterMoving = 0.4f;
    private float m_SecondsToStopTransitioning = 0.8f;
    private float m_TimerToStopTransitioning = 0f;

    private void OnEnable()
    {
        CheckpointEvents.FastCheckpointEvent += FastCheckpoint;
    }

    private void OnDisable()
    {
        CheckpointEvents.FastCheckpointEvent -= FastCheckpoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterPlayerController>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        checkpoint = GameObject.Find("Checkpoint_Ground").gameObject;

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

                    if (hit.collider != null && checkpoint != null)
                    {
                        lastSecurePosition = checkpoint.transform;

                        checkpoint.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - (this.transform.localScale.y / 2), this.gameObject.transform.position.z);
                    }
                    else if (checkpoint != null)
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
            m_TimerToStopTransitioning += Time.deltaTime;

            if(m_TimerToStopTransitioning >= m_SecondsAfterMoving)
            {
                StartCoroutine(WaitSecondsAfterMoving());
                m_TimerToStopTransitioning = 0f;
            }

            if (this.transform.position == new Vector3(checkpoint.transform.position.x, checkpoint.transform.position.y + this.transform.localScale.y / 2, checkpoint.transform.position.z))
            {
                StartCoroutine(WaitSecondsAfterMoving());
                m_TimerToStopTransitioning = 0f;
            }
        }

        /*if(Input.GetKeyDown(KeyCode.L))
        {
            FastCheckpoint();
        }*/
    }

    public void FastCheckpoint()
    {
        characterController.enabled = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        boxCollider.isTrigger = true;
        circleCollider.isTrigger = true;

        FadeToBlackEvents.eFadeToBlackAction?.Invoke(FadeIn, FadeOut);
        isTransitioning = true;
    }

    IEnumerator WaitSecondsAfterMoving()
    {
        yield return new WaitForSeconds(m_SecondsAfterMoving);

        characterController.enabled = true;
        rb.gravityScale = gravityScale;

        boxCollider.isTrigger = false;
        circleCollider.isTrigger = false;

        isTransitioning = false;
    }
}

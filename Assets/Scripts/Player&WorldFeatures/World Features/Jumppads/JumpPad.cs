using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

public class JumpPad : MonoBehaviour
{
    public float jumpForce;
    CharacterPlayerController playerController;

    public Vector2 anglePad;
    public float angle;

    private void Start()
    {
        angle = transform.rotation.eulerAngles.z;

        if (angle > 90)
        {
            angle = angle - 360;
        }

        anglePad.y = Mathf.Abs((Mathf.Abs(angle) - 90) / 90);
        anglePad.x = ((Mathf.Abs(angle)) * -Mathf.Sign(angle)) / 90;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Rigidbody2D rbPlayer = collision.GetComponent<Rigidbody2D>();
            playerController = collision.GetComponent<CharacterPlayerController>();

            rbPlayer.linearVelocity = Vector3.zero;

            if (playerController != null)
            {
                if (playerController.isDashing) //If player si Dashing when collide with JumpPad cancell Dash
                {
                    playerController.DashCancelled();
                }

                if (playerController.isImpactHitting)
                {
                    rbPlayer.AddForce(new Vector2(jumpForce * anglePad.x, (jumpForce * anglePad.y) * 1.1f));
                }
                else
                {
                    rbPlayer.AddForce(new Vector2(jumpForce * anglePad.x, jumpForce * anglePad.y));
                }
            }
        }
    }
}

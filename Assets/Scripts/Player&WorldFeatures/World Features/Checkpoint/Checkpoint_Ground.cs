using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerController;

public class Checkpoint_Ground : MonoBehaviour
{
    public GameObject checkpoint;
    CharacterPlayerController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(characterController.isGrounded)
        {
            checkpoint.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - (this.transform.localScale.y / 2), this.gameObject.transform.position.z);
        }
    }
}

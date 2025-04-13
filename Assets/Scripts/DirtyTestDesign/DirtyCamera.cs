using System;
using UnityEngine;
using UnityEngine.Rendering;

public class DirtyCamera : MonoBehaviour
{
    //Base follow
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.25f;
    public GameObject camTargetPos;

    [SerializeField] GameObject target;
    public Camera camRef; //Reference to this camera

    //Horizontal movement types
    public enum HorizonralMovement
    {
        FowardByLooking, //Hollow Knigth
        FreeCenterDirectionPull, //Blasphemous 2
        FowardByMoving //Prince of Persia
    }



    [Range(0.0f, 1.0f)]
    public float cameraDistance; //Maximun distance the camera will decenter from the player, 1 is the player is in the cam border, 0 player center of cam

    //FreeCenterDirectionPull parameters
    //[Range(0.0f, 1.0f)]
    //public float cameraMargin; //Max Distance the player can get away from camera before camera locking in.
    [SerializeField] int cameraState = 0; //0 is player is inside margin, -1 is player went to rigth so camera is displaced to the left, 1 is player went to left so camera is displaced to the rigth


    float camHeigth;
    float camWidth;

    public HorizonralMovement hMode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camHeigth = camRef.orthographicSize;
        camWidth = camRef.aspect * camHeigth;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hMode)
        {
            case HorizonralMovement.FowardByLooking:
                break;
            case HorizonralMovement.FreeCenterDirectionPull:
                break;
            case HorizonralMovement.FowardByMoving:
                break;
        }
    }

    private void LateUpdate()
    {
        float smoothModifier = 1;

        float xTarget = target.transform.position.x;
        switch (hMode)
        {
            case HorizonralMovement.FowardByLooking:
                
                if (target.GetComponent<SpriteRenderer>() != null)
                {
                    if (target.GetComponent<SpriteRenderer>().flipX) 
                    {
                        xTarget -= camWidth * cameraDistance;
                    }
                    else 
                    {
                        xTarget += camWidth * cameraDistance;
                    }
                }
                break;
            case HorizonralMovement.FreeCenterDirectionPull:
                if (cameraState == 0) 
                {
                    xTarget = this.transform.position.x;

                    if (Mathf.Abs(target.transform.position.x - this.transform.position.x) > camWidth * cameraDistance)
                    {
                        Debug.Log(Mathf.Abs(target.transform.position.x - this.transform.position.x));
                        if (target.transform.position.x - this.transform.position.x > 0)
                        {
                            cameraState = 1; //Player moving rigth
                        }
                        else if (target.transform.position.x - this.transform.position.x < 0)
                        {
                            cameraState = -1; //Player moving left
                        }
                    }
                    
                }
                else if (cameraState == 1) 
                {
                    if(target.transform.position.x - this.transform.position.x >= this.transform.position.x + camWidth * cameraDistance) 
                    {
                        //xTarget = this.transform.position.x + camWidth * cameraDistance;
                        xTarget = target.transform.position.x + camWidth * cameraDistance;
                        //smoothModifier = 0f;


                    }
                    else if(target.transform.position.x - this.transform.position.x < camWidth * cameraDistance * 0.5f)
                    {
                        cameraState = 0;
                    }
                    
                }
                else if (cameraState == -1)
                {
                    if (target.transform.position.x - this.transform.position.x <= this.transform.position.x - camWidth * cameraDistance)
                    {
                        //xTarget = this.transform.position.x - camWidth * cameraDistance;
                        xTarget = target.transform.position.x  - camWidth * cameraDistance;
                        //smoothModifier = 0f;
                    }
                    else if (target.transform.position.x - this.transform.position.x > camWidth * cameraDistance * 0.5f)
                    {
                        cameraState = 0;
                    }
                }
                break;
            case HorizonralMovement.FowardByMoving:
                break;
        }

        float yTarget = target.transform.position.y;

        Vector3 targetPosition = new Vector3
            (
            xTarget,
            yTarget,
            target.transform.position.z -10f
            );

        camTargetPos.transform.position = new Vector3(xTarget,yTarget,target.transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, (smoothTime*smoothModifier));
    }
}

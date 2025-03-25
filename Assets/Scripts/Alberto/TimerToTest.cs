using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerToTest : MonoBehaviour
{
    public float timer = 0;
    public float maxTimer = 1;

    bool startTimerHorizontal = false;
    bool startTimerVertical = false;
    bool dash = false;

    Vector3 initialPos;
    Vector3 lastPos;

    Rigidbody2D rb;

    Character_Controller Character_Controller;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = Vector3.zero;
        lastPos = Vector3.zero;

        rb = GetComponent<Rigidbody2D>();
        Character_Controller = GetComponent<Character_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            startTimerHorizontal = !startTimerHorizontal;
            initialPos = this.gameObject.transform.position;
            timer = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dash = !dash;
            initialPos = this.gameObject.transform.position;
            timer = 0;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    startTimerVertical = !startTimerVertical;
        //    initialPos = this.gameObject.transform.position;
        //    timer = 0;
        //}

        if (startTimerHorizontal)
        {
            timer += Time.deltaTime;

            if(timer >= maxTimer)
            {
                lastPos = this.gameObject.transform.position;
                Debug.Log("X:" + (lastPos.x - initialPos.x));
                startTimerHorizontal = false;
            }
        }

        if (startTimerVertical)
        {
            if (rb.linearVelocity.y < 0)
            {
                timer += Time.deltaTime;
            }

            if (rb.linearVelocity.y == 0 && timer > 0.1f)
            {
                Debug.Log("Time:" + timer);
                startTimerVertical = false;
            }
        }


        if (dash)
        {
            if (!Character_Controller.isDashing && timer > 0.05f )
            {
                lastPos = this.gameObject.transform.position;
                Debug.Log("X:" + (lastPos.x - initialPos.x));
                dash = false;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
}

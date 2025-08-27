using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class ChangeCameraBounds : MonoBehaviour
{
    public PlayerCamera cameraRef;

    [SerializeField] Rect m_newCameraBounds;
    [SerializeField] Rect m_oldCameraBounds;
    
    [System.Flags]
    enum CameraChangesMode
    {
        NONE = 0,
        ON_START = 1 << 0,   // 1
        ON_START_GO_RETURN = 1 << 1,     // 2
        ON_COLLISION_ONCE = 1 << 2,   // 4
        ON_COLLISION_STAY = 1 << 3,    // 8
        WALL = 1 << 4,           // 16
        ALL = ON_START | ON_START_GO_RETURN | ON_COLLISION_ONCE | ON_COLLISION_STAY | WALL
    }

    [Header("Change Modifiers")]
    [SerializeField] CameraChangesMode modes;
    [SerializeField] float m_timeForChange = 0;
    float timePassed = 0;
    [TagDropdown] public string[] collisionTag = new string[] { };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if ((modes & (CameraChangesMode.ON_START | CameraChangesMode.ON_START_GO_RETURN)) != 0) 
        {
            StartCoroutine(ChangeBounds());

            if ((modes & (CameraChangesMode.ON_START_GO_RETURN)) != 0) 
            {
                StartCoroutine(ChangeBounds(m_timeForChange + 0.1f,true));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((modes & (CameraChangesMode.ON_COLLISION_ONCE | CameraChangesMode.ON_COLLISION_STAY)) != 0) 
        {
            if (collisionTag.Contains(collision.gameObject.tag))
            {
                StopAllCoroutines();
                StartCoroutine(ChangeBounds(m_timeForChange));

                if ((modes & (CameraChangesMode.ON_COLLISION_ONCE)) != 0) // If on collision once, makes sure doesn't repeat
                {
                    modes = CameraChangesMode.NONE;
                }
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((modes & (CameraChangesMode.ON_COLLISION_STAY)) != 0)
        {
             if (collisionTag.Contains(collision.gameObject.tag))
            {
                StopAllCoroutines();
                StartCoroutine(ChangeBounds(m_timeForChange, true));
            }
        }
    }

    // Separation for code comodity
    IEnumerator ChangeBounds(float waitToExecute = 0.0f,bool reverseOrigin = false)
    {
        yield return new WaitForSeconds(waitToExecute);
        if (reverseOrigin) { m_newCameraBounds = m_oldCameraBounds; }
        m_oldCameraBounds = cameraRef.cameraBounds;
        timePassed = 0.0f;

        while (timePassed < m_timeForChange)
        {
            // Lerp
            cameraRef.cameraBounds.x = m_oldCameraBounds.x * 1- ((timePassed / m_timeForChange)) + m_newCameraBounds.x * (timePassed / m_timeForChange);
            cameraRef.cameraBounds.y = m_oldCameraBounds.y * (1 - (timePassed / m_timeForChange)) + m_newCameraBounds.y * (timePassed / m_timeForChange);
            cameraRef.cameraBounds.width = m_oldCameraBounds.width * (1 - (timePassed / m_timeForChange)) + m_newCameraBounds.width * (timePassed / m_timeForChange);
            cameraRef.cameraBounds.height = m_oldCameraBounds.height * (1 - (timePassed / m_timeForChange)) + m_newCameraBounds.height * (timePassed / m_timeForChange);
            timePassed += Time.deltaTime;

            Debug.Log(cameraRef.cameraBounds);

            // Yield here
            yield return null;
        }

        // Make sure we got there
        cameraRef.cameraBounds = m_newCameraBounds;
        timePassed = 0;
        yield return null;
    }


}

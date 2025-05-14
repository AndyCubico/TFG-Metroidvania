using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerCamera : MonoBehaviour
{
    // Camera reference
    private Camera m_Camera;
    private float m_CameraHeight; // Camera heigth from the centre, half of total heigth
    private float m_CameraWidth; // Camera width from the centre, half of total width

    // Camera movement
    [Header("Basic Parameters")]
    [SerializeField] private GameObject m_Target; // Game Object the camera is following,usually the player
    public Rect cameraBounds;
    enum TargetDirection 
    {
        MOVING_LEFT = -1, // Player moving left, shift camera focus towards rigth
        STATIC = 0, //Player not moving, shifting camera focus towards centre
        MOVING_RIGTH = 1, // Player moving rigth, shift camera focus towards left
    }
    private TargetDirection m_TargetMovementDirection = TargetDirection.STATIC;

    private float m_TimeForUpdate = 0.0f;
    [Header("Threshold Parameters")]
    [SerializeField] private float m_TimeBetweenPositionUpdates;
    [SerializeField] private float m_DistanceThreshold; // Minimum distance the target must move for m_TargetMovementDirection to change.
    private Vector3 m_LastPositionTarget;

    // Camera "Threshold triggered dual-forward-focus" movement
    [SerializeField] private float m_MaximunDistanceTreshold; // Value between 0-1, determines the percentatge of distance the player is placed off-center of the camera.
    [SerializeField] private float m_TimeToReachThreshold; // Time that the camera takes to reach from 0 to 1 or -1 in m_CurrentDistanceTreshold.
    private float m_CurrentDistanceTreshold; // Value where the player is at the moment in comprasion to the camera centre. Value is betwenn -1 (left) and 1 (rigth)

    // Camera debug
    private bool m_IsDebugMode = false; 
    [Header("Debug")]
    [SerializeField] private GameObject m_CenterCamera;
    [SerializeField] private GameObject m_HorizontalTargetLeft;
    [SerializeField] private GameObject m_HorizontalTargetRigth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Camera = gameObject.GetComponent<Camera>();
        m_CameraHeight = m_Camera.orthographicSize;
        m_CameraWidth = m_Camera.aspect * m_CameraHeight;

        if (m_Target ??= GameObject.FindGameObjectWithTag("Player")) 
        {
            m_LastPositionTarget = m_Target.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // After a certain amount of time, check if position has changed
        m_TimeForUpdate += Time.deltaTime;

        if (m_TimeForUpdate > m_TimeBetweenPositionUpdates)
        {
            m_TimeForUpdate = 0.0f;
            if (Mathf.Abs(m_LastPositionTarget.x - m_Target.transform.position.x) <= m_DistanceThreshold)
            {
                m_TargetMovementDirection = TargetDirection.STATIC;
            }
            else
            {
                if (m_LastPositionTarget.x < m_Target.transform.position.x)
                {
                    m_TargetMovementDirection = TargetDirection.MOVING_RIGTH;
                }
                else
                {
                    m_TargetMovementDirection = TargetDirection.MOVING_LEFT;
                }
            }

            m_LastPositionTarget = m_Target.transform.position;
        }
        // Every frame update target focus due to the horizontal 
        switch ((m_TargetMovementDirection)
)
        {
            case TargetDirection.MOVING_LEFT:

                m_CurrentDistanceTreshold += Time.deltaTime / m_TimeToReachThreshold;
                m_CurrentDistanceTreshold = Mathf.Min(1, m_CurrentDistanceTreshold);

                break;
            case TargetDirection.STATIC:

                if (m_CurrentDistanceTreshold != 0) // Once the camera is perfectly centered in the characther it must stop tring to center, otherwise, it shakes
                {
                    m_CurrentDistanceTreshold -= Mathf.Sign(m_CurrentDistanceTreshold) * Time.deltaTime / m_TimeToReachThreshold;
                    if(Mathf.Abs(m_CurrentDistanceTreshold) <= (Time.deltaTime / m_TimeToReachThreshold)) // If we are very near to 0, make the value 0. 
                    {
                        m_CurrentDistanceTreshold = 0;
                    }
                }
                

                break;
            case TargetDirection.MOVING_RIGTH:

                m_CurrentDistanceTreshold -= Time.deltaTime / m_TimeToReachThreshold;
                m_CurrentDistanceTreshold = Mathf.Max(-1, m_CurrentDistanceTreshold);
                break;
            default:
                break;
        }
        // Debug things
        if (m_IsDebugMode)
        {
            m_CenterCamera.transform.position = transform.position;
            //m_HorizontalTarget.transform.position = new Vector3((transform.position.x * (1-m_CurrentDistanceTreshold) + m_CameraWidth*m_MaximunDistanceTreshold*m_CurrentDistanceTreshold),transform.position.y,transform.position.z);
            m_HorizontalTargetLeft.transform.position = new Vector3(m_Target.transform.position.x - (-m_CameraWidth * m_MaximunDistanceTreshold), transform.position.y, transform.position.z);
            m_HorizontalTargetRigth.transform.position = new Vector3(m_Target.transform.position.x - (m_CameraWidth * m_MaximunDistanceTreshold), transform.position.y, transform.position.z);
        }

        if(Input.GetKeyUp(KeyCode.F4)) 
        {
            // Inverse m_IsDebugMode and change active state all elements.
            m_IsDebugMode = !m_IsDebugMode;
            SetDebugStatus(m_IsDebugMode);
        }
    }

    private void LateUpdate()
    {
        // Camera position
        float targetPositionX = 0;
        float targetPositionY = 0;
        float targetPositionZ = gameObject.transform.position.z;

        //Threshold triggered dual-foward-focus (X axis)
        targetPositionX = m_Target.transform.position.x - (0 * (1 - m_CurrentDistanceTreshold) + m_CameraWidth * m_MaximunDistanceTreshold * m_CurrentDistanceTreshold);


        // Edge snapping
        targetPositionX = Mathf.Min(cameraBounds.width - m_CameraWidth, targetPositionX);
        targetPositionX = Mathf.Max(cameraBounds.x + m_CameraWidth, (targetPositionX));
        targetPositionY = Mathf.Min(cameraBounds.height - m_CameraHeight, targetPositionY);
        targetPositionY = Mathf.Max(cameraBounds.y + m_CameraHeight, (targetPositionY));

        gameObject.transform.position = new Vector3(targetPositionX, targetPositionY, targetPositionZ);
    }

    public bool SetDebugStatus(bool active) 
    {
        m_CenterCamera.SetActive(active);
        m_HorizontalTargetLeft.SetActive(active);
        m_HorizontalTargetRigth.SetActive(active);

        return active;
    }
}

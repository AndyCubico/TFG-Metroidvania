using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Camera reference
    private Camera m_Camera;
    private float m_CameraHeight;
    private float m_CameraWidth;

    // Camera movement
    [Header("Basic Parameters")]
    [SerializeField] private GameObject m_Target;
    public Rect cameraBounds;
    enum TargetDirection 
    {
        MOVING_LEFT = -1,
        STATIC = 0,
        MOVING_RIGHT = 1,
    }
    private TargetDirection m_TargetMovementDirection = TargetDirection.STATIC;

    [Header("Threshold Parameters")]
    private float m_TimeForUpdate = 0.0f;
    [SerializeField] private float m_TimeBetweenPositionUpdates;
    [SerializeField] private float m_DistanceThreshold; // Minimum distance the target must move for m_TargetMovementDirection to change.
    private float m_LastPositionXTarget;


    // Camera debug
    private bool m_IsDebugMode = false;
    [SerializeField] private GameObject m_CenterCamera;
    [SerializeField] private GameObject m_VerticalTarget;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Camera = gameObject.GetComponent<Camera>();
        m_CameraHeight = m_Camera.orthographicSize;
        m_CameraWidth = m_Camera.aspect * m_CameraHeight;

        if (m_Target ??= GameObject.FindGameObjectWithTag("Player")) 
        {
            m_LastPositionXTarget = m_Target.transform.position.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // After a certain amount of time, check if position has changed
        if (m_TimeForUpdate > m_TimeBetweenPositionUpdates) 
        {
            m_TimeForUpdate = 0.0f;
            if (Mathf.Abs(m_LastPositionXTarget-m_Target.transform.position.x) <= m_DistanceThreshold) 
            {
                m_TargetMovementDirection = TargetDirection.STATIC;
            }
            else 
            {
                if(m_LastPositionXTarget)
            }

        }

        if (m_IsDebugMode)
        {
            m_CenterCamera.transform.position = transform.position;
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
        m_VerticalTarget.SetActive(active);

        return active;
    }
}

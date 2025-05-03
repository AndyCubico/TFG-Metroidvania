using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Pathfollowing : MonoBehaviour
{
    private NativeList<int2> m_Path;
    private int m_PathIndex = 0; // Index to keep track of the next step to follow.
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private bool m_IsGrounded;

    private Rigidbody2D m_rb;
    [SerializeField] private LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Vector3 m_PreviousPosition;

    // Debug
    [SerializeField] private int2 m_StartPosition;
    [SerializeField] private int2 m_EndPosition;

    private void Awake()
    {
        m_Path = new NativeList<int2>(Allocator.Persistent);
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPath(m_StartPosition, m_EndPosition);
        }

        if (m_PathIndex >= 0 && !m_Path.IsEmpty)
        {
            Vector3 targetPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);
            Vector3 moveDirection = targetPosition - transform.position;

            Debug.Log("TARGET: " + targetPosition);
            Debug.Log("CURRENT: " + transform.position);
            Debug.Log("DIRECTION: " + moveDirection);

            if (CheckJump(targetPosition))
            {
                // Jump
                Jump();
            }
            else
            {
                MoveToX(moveDirection);
            }

            if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.1f)
            {
                m_PreviousPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);

                // Go to next index
                m_PathIndex--;
            }
        }
    }

    private void MoveToX(Vector3 direction)
    {
        int moveDir = MathF.Sign(direction.x);
        m_rb.linearVelocityX = m_Speed * Time.deltaTime;
    }

    private void Jump()
    {
        m_rb.AddForceY(m_JumpForce, ForceMode2D.Impulse);
    }

    private bool CheckJump(Vector3 targetPosition)
    {
        return targetPosition.y > m_PreviousPosition.y + 0.1f && CheckIsGrounded(); // Add 0.1f to avoid jumping when the difference in y is too small. 
    }

    private bool CheckIsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Set the path for the agent to follow.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void SetPath(int2 start, int2 end)
    {
        NativeList<int2> path = new NativeList<int2>(Allocator.TempJob);

        PathfindingManager.Instance.StartPathfinding(path, start, end);

        if (m_Path.IsCreated)
            m_Path.Clear();

        for (int i = 0; i < path.Length; i++)
        {
            m_Path.Add(path[i]);
        }

        m_PathIndex = m_Path.Length - 1;
        m_PreviousPosition = new Vector3(m_Path[m_Path.Length - 1].x + 0.5f, m_Path[m_Path.Length - 1].y + 0.5f, 0);

        path.Dispose();
    }

    private void OnDestroy()
    {
        if (m_Path.IsCreated)
            m_Path.Dispose();
    }
}

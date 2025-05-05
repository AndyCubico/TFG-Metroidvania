using System;
using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Pathfollowing : MonoBehaviour
{
    private NativeList<int2> m_Path;
    private int m_PathIndex = 0; // Index to keep track of the next step to follow.
    private Vector3 m_TargetPosition;
    private Vector3 m_MoveDirection;

    [Header("Movement stats")]
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_JumpForce;

    [Header("Collision management")]
    private Rigidbody2D m_rb;
    [SerializeField] private LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Jump management")]
    private Vector3 m_PreviousPosition;
    [SerializeField] private float m_JumpWait = 1.0f;
    [SerializeField] private bool m_IsGrounded;

    // TODO: REWORK, TOO DIRTY
    private bool m_IsJumping = false;
    private bool m_CoroutineExecution = false;

    private void Awake()
    {
        m_Path = new NativeList<int2>(Allocator.Persistent);
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GridManager.Instance.grid.GetXYPosition(mouseWorldPos, out int x, out int y);
            GridNode node = GridManager.Instance.grid.GetValue(x, y);

            Debug.Log($"Origin node is ({Mathf.FloorToInt(transform.position.x)}, {Mathf.FloorToInt(transform.position.y)})");

            Debug.Log($"Destination node is ({x}, {y})");

            if (node != null)
            {
                SetPath(new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)), new int2(x, y));
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_PathIndex >= 0 && !m_Path.IsEmpty)
        {
            m_TargetPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);
            m_MoveDirection = m_TargetPosition - transform.position;

            if (!m_CoroutineExecution)
            {
                if (CheckJump(m_TargetPosition))
                {
                    StartCoroutine(Jump(m_JumpWait));
                }
                else if (!m_IsJumping)
                {
                    MoveToX(m_MoveDirection);
                }
                else
                {
                    m_IsJumping = !CheckIsGrounded();
                }
            }

            if (Mathf.Abs(transform.position.x - m_TargetPosition.x) < 0.1f)
            {
                m_PreviousPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);

                // Go to next index
                m_PathIndex--;

                if (m_PathIndex == -1)
                {
                    m_rb.linearVelocityX = 0;
                }
            }
        }
    }

    private void MoveToX(Vector3 direction)
    {
        m_rb.linearVelocityX = m_Speed * MathF.Sign(direction.x);
    }

    private IEnumerator Jump(float waitTime)
    {
        m_IsJumping = true;
        m_CoroutineExecution = true;

        //// Go to jump position, which is the last node in the path that was reached.
        if (Mathf.Abs(transform.position.x - m_PreviousPosition.x) > 0.01f)
        {
            Vector3 targetPosition = new Vector3(m_PreviousPosition.x, m_PreviousPosition.y, 0);
            Vector3 moveDirection = targetPosition - transform.position;
            m_rb.linearVelocityX = m_Speed * MathF.Sign(moveDirection.x);
            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        m_rb.linearVelocity = Vector2.zero;
        m_rb.angularVelocity = 0f;

        // Make agent wait a X seconds to perform the jump
        //yield return new WaitForSeconds(waitTime);

        //m_rb.linearVelocityX = m_Speed * MathF.Sign(m_MoveDirection.x); // Reset direction.
        m_rb.AddForce(new Vector2(m_JumpForce / 2, m_JumpForce), ForceMode2D.Impulse); // Apply jump force.
    }

    private bool CheckJump(Vector3 targetPosition)
    {
        return targetPosition.y > m_PreviousPosition.y + 0.1f && CheckIsGrounded(); // Add 0.1f to avoid jumping when the difference in y is too small. 
    }

    private bool CheckIsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
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

        // Make sure path is valid.
        if (path.Length != 0)
        {
            for (int i = 0; i < path.Length; i++)
            {
                m_Path.Add(path[i]);
            }

            m_PathIndex = m_Path.Length - 1;
            m_PreviousPosition = new Vector3(m_Path[m_Path.Length - 1].x + 0.5f, m_Path[m_Path.Length - 1].y + 0.5f, 0);
        }

        path.Dispose();
    }

    // TODO: rework into components.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && m_CoroutineExecution)
        {
            m_CoroutineExecution = false;
        }
    }

    private void OnDestroy()
    {
        if (m_Path.IsCreated)
            m_Path.Dispose();
    }
}

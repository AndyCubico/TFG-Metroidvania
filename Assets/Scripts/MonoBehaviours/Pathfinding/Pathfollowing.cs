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

    [Header("Jump management")]
    private Vector3 m_PreviousPosition;
    [SerializeField] private float m_JumpWait = 1.0f;
    [SerializeField] private bool m_IsGrounded;

    // TODO: If check radius ends up not changing between checkers, delete variables and use only one.
    [Header("Collision management")]
    private Rigidbody2D m_rb;
    [SerializeField] private LayerMask m_GroundLayer;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private float m_GroundCheckRadius = 0.1f;

    // Jump in cliff
    [SerializeField] private Transform m_RightCliffCheck;
    [SerializeField] private float m_RightCliffCheckRadius = 0.1f;
    [SerializeField] private Transform m_LeftCliffCheck;
    [SerializeField] private float m_LeftCliffCheckRadius = 0.1f;

    private Helper.Int2Comparer m_Comparer;

    [Header("Stuck management")]
    // Manage the agent if it gets stuck.
    private float m_Timer = 0.0f;
    [SerializeField] private float m_StuckTime = 2.0f;
    private int m_LastPathIndex = 0;

    // TODO: REWORK USING COMPONENTS
    private bool m_IsJumping = false;
    private bool m_CoroutineExecution = false;

    private void Awake()
    {
        m_Path = new NativeList<int2>(Allocator.Persistent);

        m_rb = GetComponent<Rigidbody2D>();

        m_Comparer = new Helper.Int2Comparer();
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

        DrawPath();
    }

    private void FixedUpdate()
    {
        if (m_PathIndex >= 0 && !m_Path.IsEmpty)
        {
            m_TargetPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);
            m_MoveDirection = m_TargetPosition - transform.position;
            bool isCliff = GridManager.Instance.grid.GetValue(Mathf.FloorToInt(m_Path[m_PathIndex].x), m_Path[m_PathIndex].y).IsCliff();

            if (!m_CoroutineExecution)
            {
                if (CheckJump(m_TargetPosition))
                {
                    StartCoroutine(Jump(m_JumpWait));
                }
                else if (isCliff && CheckIsGrounded(m_GroundCheck, m_GroundCheckRadius) &&
                    (!CheckIsGrounded(m_RightCliffCheck, m_RightCliffCheckRadius) ||
                    !CheckIsGrounded(m_LeftCliffCheck, m_LeftCliffCheckRadius)))
                {
                    StartCoroutine(Jump(m_JumpWait, 0.5f));
                }
                else if (!m_IsJumping)
                {
                    MoveToX(m_MoveDirection);
                }
                else
                {
                    m_IsJumping = !CheckIsGrounded(m_GroundCheck, m_GroundCheckRadius);
                }

                if (m_PathIndex == m_LastPathIndex)
                {
                    m_Timer += Time.deltaTime;

                    // If the agent is stucked, cancel path and return to origin.
                    if (m_Timer >= m_StuckTime)
                    {
                        m_Timer = 0;

                        if (m_PathIndex + 1 < m_Path.Length)
                        {
                            SetPath(new int2(m_Path[m_PathIndex + 1].x, m_Path[m_PathIndex + 1].y), new int2(m_Path[m_Path.Length - 1].x, m_Path[m_Path.Length - 1].y));
                        }
                    }
                }
            }

            if (Mathf.Abs(transform.position.x - m_TargetPosition.x) < 0.1f)
            {
                m_PreviousPosition = new Vector3(m_Path[m_PathIndex].x + 0.5f, m_Path[m_PathIndex].y + 0.5f, 0);

                // Go to next index
                m_PathIndex--;
                m_LastPathIndex = m_PathIndex;

                // Reset timer to check if stuck.
                m_Timer = 0;

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

    private IEnumerator Jump(float waitTime, float forceX = 0.35f)
    {
        Debug.Log("JUMP");

        m_IsJumping = true;
        m_CoroutineExecution = true;

        yield return new WaitForSeconds(waitTime / 2);

        Vector3 targetPosition = new Vector3(m_PreviousPosition.x, m_PreviousPosition.y, 0);
        Vector3 moveDirection = targetPosition - transform.position;

        // Go to jump position, which is the last node in the path that was reached.
        if (Mathf.Abs(transform.position.x - m_PreviousPosition.x) > 0.01f)
        {
            m_rb.linearVelocityX = m_Speed * MathF.Sign(moveDirection.x);
            yield return new WaitForSeconds(waitTime);
        }
        //else
        //{
        //    yield return new WaitForSeconds(waitTime);
        //}

        m_rb.linearVelocity = Vector2.zero;
        m_rb.angularVelocity = 0f;

        m_rb.AddForce(new Vector2(Mathf.Sign(m_MoveDirection.x) * m_JumpForce * forceX, m_JumpForce), ForceMode2D.Impulse); // Apply jump force.
    }

    private bool CheckJump(Vector3 targetPosition)
    {
        return targetPosition.y > m_PreviousPosition.y + 0.1f && CheckIsGrounded(m_GroundCheck, m_GroundCheckRadius); // Add 0.1f to avoid jumping when the difference in y is too small. 
    }

    private bool CheckIsGrounded(Transform check, float radius)
    {
        return Physics2D.OverlapCircle(check.position, radius, m_GroundLayer);
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

        // Delete starting position if it is the same of the previous path taken,
        // to avoid the agent restarting their path each time the target changes destination.
        if (m_Path.Length != 0 && path.Length != 0 && m_Comparer.Equals(m_Path[m_Path.Length - 1], path[path.Length - 1]))
        {
            path.RemoveAt(path.Length - 1);
        }

        if (m_Path.IsCreated)
            m_Path.Clear();

        // Make sure path is valid. If there is no path or the final position is a cliff, invalid.
        if (path.Length != 0 && !GridManager.Instance.grid.GetValue(Mathf.FloorToInt(path[0].x), path[0].y).IsCliff())
        {
            for (int i = 0; i < path.Length; i++)
            {
                m_Path.Add(path[i]);
            }

            m_PathIndex = m_Path.Length - 1;
            m_LastPathIndex = m_PathIndex;
            m_PreviousPosition = new Vector3(m_Path[m_Path.Length - 1].x + 0.5f, m_Path[m_Path.Length - 1].y + 0.5f, 0);
        }
        else
        {
            Debug.Log("Path is not valid.");
        }

        path.Dispose();
    }

    private void OnDrawGizmos()
    {
        if (m_GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundCheckRadius);
        }

        if (m_RightCliffCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_RightCliffCheck.position, m_RightCliffCheckRadius);
        }

        if (m_LeftCliffCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_LeftCliffCheck.position, m_LeftCliffCheckRadius);
        }
    }

    private void DrawPath()
    {
        if (m_Path.Length != 0)
        {
            for (int i = m_PathIndex; i > 0; i--)
            {
                Vector3 worldStart = new Vector3(m_Path[i].x + 0.5f, m_Path[i].y + 0.5f, 0);
                Vector3 worldEnd = new Vector3(m_Path[i - 1].x + 0.5f, m_Path[i - 1].y + 0.5f, 0);
                Debug.DrawLine(worldStart, worldEnd, Color.green);

                Debug.DrawRay(worldStart, Vector3.up * 0.5f, Color.yellow);
            }
        }
    }

    // TODO: Rework into components.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_GroundLayer) != 0 && m_CoroutineExecution)
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

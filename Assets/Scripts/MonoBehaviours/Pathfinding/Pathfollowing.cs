using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Pathfollowing : MonoBehaviour
{
    private NativeList<int2> m_Path;
    private int m_PathIndex = 0; // Index to keep track of the next step to follow.
    [SerializeField] private float m_Speed;

    private void Awake()
    {
        m_Path = new NativeList<int2>(Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetPath(new int2(0, 0), new int2(3, 1));
        }

        if (m_PathIndex >= 0 && !m_Path.IsEmpty)
        {
            Vector3 targetPosition = new Vector3(m_Path[m_PathIndex].x, m_Path[m_PathIndex].y, 0);
            Vector3 moveDirection = math.normalizesafe(targetPosition - transform.position);

            transform.position += moveDirection * m_Speed * Time.deltaTime;

            if (math.distance(transform.position, targetPosition) < 0.1f)
            {
                // Go to next index
                m_PathIndex--;
            }
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

        for (int i = 0; i < path.Length; i++)
        {
            m_Path.Add(path[i]);
        }

        m_PathIndex = m_Path.Length - 1;

        path.Dispose();
    }

    private void OnDestroy()
    {
        if (m_Path.IsCreated)
            m_Path.Dispose();
    }
}

using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Wander", menuName = "Enemy Logic/Idle/Idle Wander")]
public class IdleWander : IdleSOBase
{
    [SerializeField] float m_MovementSpeed;
    [SerializeField] float m_IdleWaitTime = 2.0f;

    private int2 m_Destination = new int2();
    private float m_Timer = 0f;
    private bool m_IsWaiting = false;

    public override void DoEnter()
    {
        base.DoEnter();
        PickNewDestination();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();

        // Check if path has finished
        if (enemy.pathfollowing.IsPathFinished())
        {
            if (!m_IsWaiting)
            {
                m_IsWaiting = true;
                m_Timer = 0f;
            }

            m_Timer += Time.fixedDeltaTime;

            if (m_Timer >= m_IdleWaitTime)
            {
                PickNewDestination();
                m_IsWaiting = false;
            }
        }
    }

    private void PickNewDestination()
    {
        m_Destination = SetRandomDestination();

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        enemy.pathfollowing.SetPath(currentPos, m_Destination);
    }

    private int2 SetRandomDestination()
    {
        int radius = 5;

        int2 current = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 destination;

        GridNode node;

        do
        {
            destination = new int2(
                current.x + UnityEngine.Random.Range(-radius, radius + 1),
                current.y + UnityEngine.Random.Range(-radius, radius + 1)
            );

            node = GridManager.Instance.grid.GetValue(destination.x, destination.y);
        }
        while (node == null || !node.IsWalkable() || node.IsCliff());

        return destination;
    }
}

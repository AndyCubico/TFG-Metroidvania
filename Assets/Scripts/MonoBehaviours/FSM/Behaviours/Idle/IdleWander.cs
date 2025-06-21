using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Wander", menuName = "Enemy Logic/Idle/Idle Wander")]
public class IdleWander : IdleSOBase
{
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_IdleWaitTime = 2.0f;

    [Header ("Wander range")]
    [SerializeField] private int m_Radius = 5;

    private int2 m_Destination = new int2();
    private float m_Timer = 0f;
    private bool m_IsWaiting = false;

    public override void DoEnter()
    {
        base.DoEnter();

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 targetPos = new int2(Mathf.FloorToInt(2.5f), Mathf.FloorToInt(4.71f));

        enemy.pathfollowing.SetPath(currentPos, targetPos);
    }
    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        // DEBUG
        if (Input.GetKeyDown(KeyCode.T))
        {
            enemy.stateMachine.Transition(enemy.chaseState);
        }
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

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    private void PickNewDestination()
    {
        m_Destination = SetRandomXDestination();

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        enemy.pathfollowing.SetPath(currentPos, m_Destination);
    }

    private int2 SetRandomXDestination()
    {
        int2 current = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 destination;

        GridNode node;

        int attempts = 0;

        do
        {
            int offsetX = UnityEngine.Random.Range(-m_Radius, m_Radius + 1);
            destination = new int2(current.x + offsetX, current.y); 

            node = GridManager.Instance.grid.GetValue(destination.x, destination.y);
            attempts++;

            if (attempts > 20) break; // Avoid infinite loops.
        }
        while (node == null || !node.IsWalkable() || node.IsCliff());

        return destination;
    }
}

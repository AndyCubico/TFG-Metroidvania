using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Static Idle", menuName = "Enemy Logic/Idle/Static Idle")]
public class StaticIdle : IdleSOBase
{
    [SerializeField]
    private float m_CheckInterval = 5f; // Interval in seconds, settable in inspector
    private float m_TimeSinceLastCheck = 0f;

    private int2 m_StartPosition = new int2();

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);

        m_StartPosition = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        // For tall enemies like the elite, since the starting position has to be the floor the transform.position
        // in a large enemy could be in the node aobve. This ensures it still takes the floor.
        if (!GridManager.Instance.grid.GetValue(m_StartPosition.x, m_StartPosition.y).IsWalkable())
        {
            m_StartPosition = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - 1)); ;
        }

        m_TimeSinceLastCheck = 0f;
    }

    public override void DoEnter()
    {
        base.DoEnter();

        m_TimeSinceLastCheck = 0f;
        CheckReturnToStart();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        m_TimeSinceLastCheck += Time.deltaTime;

        if (m_TimeSinceLastCheck >= m_CheckInterval)
        {
            CheckReturnToStart();
            m_TimeSinceLastCheck = 0f;
        }
    }

    private void CheckReturnToStart()
    {
        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        if (!currentPos.Equals(m_StartPosition))
        {
            enemy.pathfollowing.SetPath(currentPos, m_StartPosition);
        }
    }
}

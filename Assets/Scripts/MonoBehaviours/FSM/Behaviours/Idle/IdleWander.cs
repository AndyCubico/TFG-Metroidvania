using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Wander", menuName = "Enemy Logic/Idle/Idle Wander")]
public class IdleWander : IdleSOBase
{
    [SerializeField] float m_MovementSpeed;

    private int2 m_Destination = new int2();

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        base.DoEnter();

        m_Destination = SetRandomDestination();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();

        // Pathfinding to raondom position, call SetRandomDestination when path is finished
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
        
    private int2 SetRandomDestination()
    {
        int2 destination = new int2();

        return destination;
    }
}

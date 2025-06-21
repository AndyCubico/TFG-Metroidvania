using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase Melee", menuName = "Enemy Logic/Chase/Chase Melee")]
public class ChaseMelee : ChaseSOBase
{
    [SerializeField] float m_ChaseSpeed;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        base.DoEnter();

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 targetPos = new int2(Mathf.FloorToInt(playerTransform.position.x), Mathf.FloorToInt(playerTransform.position.y));

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
        if (enemy.pathfollowing.IsPathFinished())
        {
            enemy.stateMachine.Transition(enemy.idleState);
        }
    }

    public override void DoFixedUpdate()
    {
        // Set pathfinding route every X seconds
        base.DoFixedUpdate();
    }

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

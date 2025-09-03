using UnityEngine;

[CreateAssetMenu(fileName = "Chase Static", menuName = "Enemy Logic/Chase/Chase Static")]
public class ChaseStatic : ChaseSOBase
{
    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void DoEnter()
    {
        base.DoEnter();

        // Not in the base class because some enemies may not have a pathfollowing behaviour.
        enemy.pathfollowing.CancelJump();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (!enemy.isInSensor)
        {
            enemy.stateMachine.Transition(enemy.idleState);
            enemy.SetTransitionAnimation("Idle");
            Debug.Log("CHASE --> IDLE");
        }
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

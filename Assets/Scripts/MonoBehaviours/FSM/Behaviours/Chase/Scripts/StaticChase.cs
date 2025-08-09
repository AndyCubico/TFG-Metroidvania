using UnityEngine;

[CreateAssetMenu(fileName = "Static Chase", menuName = "Enemy Logic/Chase/Static Chase")]
public class StaticChase : ChaseSOBase
{
    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void DoEnter()
    {
        base.DoEnter();
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

using UnityEngine;

public class IdleState : State
{
    public IdleState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void AnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.AnimationTrigger(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.isTriggered)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

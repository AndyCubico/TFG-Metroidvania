using UnityEngine;
using Unity.Mathematics;

public class ChaseState : State
{
    private int2 m_TargetPosition; // Get the player position.

    public ChaseState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void AnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.AnimationTrigger(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entering Chase State");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    // TODO: call the SetPath function in Pathfollowing.cs
    public override void Update()
    {
        base.Update();

        if (enemy.isWithinRange)
        {
            enemy.stateMachine.Transition(enemy.attackState);
        }

        // TODO: check with a timer if the player is too far away with no line of sight, return to idle.
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

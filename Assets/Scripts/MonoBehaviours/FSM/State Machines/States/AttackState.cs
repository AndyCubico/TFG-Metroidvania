using UnityEngine;
using UnityEngine.Rendering;

public class AttackState : State
{
    // Manage how long it takes to exit the state.
    private float m_ExitTimer;
    private float m_TimeToExit = 3.0f;
    private float m_DistanceLimit = 3.0f;

    // Manage how much time it takes between attacks.
    private float m_AttackTimer;
    private float m_TimeToAttack = 3.0f;

    private Transform m_PlayerTransform;

    public AttackState(Enemy enemy, StateMachine stateMachine) : base(enemy, stateMachine)
    {
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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

        // TODO: check with a timer if the player is too far away to attack, return to chase.
        if (m_AttackTimer >= m_TimeToAttack)
        {
            // Attack
        }

        if (/* Player too far away */true)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
        }

        m_AttackTimer += Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

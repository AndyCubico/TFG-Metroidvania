using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Include all the states that this enemy should have
    public IdleState idleState { get; set; }
    public ChaseState chaseState { get; set; }
    public AttackState attackState { get; set; }

    protected override void Awake()
    {
        base.Awake();

        // Initialize states
        idleState = new IdleState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);

        // Register all states
        stateRegistry[typeof(IdleState)] = idleState;
        stateRegistry[typeof(ChaseState)] = chaseState;
        stateRegistry[typeof(AttackState)] = attackState;
    }


    protected override void Start()
    {
        base.Start();

        // Call starting state of the State Machine
        stateMachine.Initialize(idleState);
    }

    public override void Move(Vector2 velocity)
    {
        base.Move(velocity);
    }

    public override void PerformAttack()
    {
        base.PerformAttack();
    }
}

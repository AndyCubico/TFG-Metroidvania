using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovement, ITrigger
{
    [field: SerializeField] public float m_MaxHealth { get; set; } = 100f;
    
    public float currentHealth { get; set; }
    public Pathfollowing pathfollowing { get; set; }

    public bool isTriggered { get; set; }
    public bool isWithinRange { get; set; }

    #region State Machine variables

    public StateMachine stateMachine { get; set; }
    public IdleState idleState { get; set; }
    public ChaseState chaseState { get; set; }
    public AttackState attackState { get; set; }

    #endregion

    #region Idle State variables


    #endregion

    private void Awake()
    {
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);
    }

    void Start()
    {
        currentHealth = m_MaxHealth;
        pathfollowing = GetComponent<Pathfollowing>();

        // Call starting state of the State Machine
        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.FixedUpdate();
    }

    #region Health & Die functions

    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Movement Functions

    public void Move(Vector2 velocity)
    {
        // TODO: Connect with pathfinding
        // Add check right or left functionality
    }

    #endregion

    #region Distance check Functions
    public void SetChaseStatus(bool isTriggered)
    {
        this.isTriggered = isTriggered;
    }

    public void SetAttackDistance(bool isWithinRange)
    {
        this.isWithinRange = isWithinRange;
    }

    #endregion

    #region Animation Triggers

    private void AnimationTrigger(ANIMATION_TRIGGER triggerType)
    {
        // TODO: In the given animation, add the trigger calling this function with the animation type needed
        stateMachine.CurrentState.AnimationTrigger(triggerType);
    }

    public enum ANIMATION_TRIGGER
    {
        ENEMYDAMAGED,
        IDLE,
        WALK
    }

    #endregion
}

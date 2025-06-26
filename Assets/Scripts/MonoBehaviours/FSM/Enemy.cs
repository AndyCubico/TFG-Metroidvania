using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovement, ITrigger
{
    #region Damagable variables

    [field: SerializeField] public float m_MaxHealth { get; set; } = 100f;
    public float currentHealth { get; set; }

    #endregion

    #region Movement variables

    public Pathfollowing pathfollowing { get; set; }
    public bool isFacingRight { get ; set; } = true; // Default sprite is facing right

    #endregion

    #region Trigger variables

    public bool isInSensor { get; set; }
    public bool isWithinAttackRange { get; set; }
    
    #endregion

    #region State Machine variables

    public StateMachine stateMachine { get; set; }
    public IdleState idleState { get; set; }
    public ChaseState chaseState { get; set; }
    public AttackState attackState { get; set; }

    #endregion

    #region Scriptable object variables

    [SerializeField] private IdleSOBase m_IdleSOBase;
    [SerializeField] private ChaseSOBase m_ChaseSOBase;
    [SerializeField] private AttackSOBase m_AttackSOBase;

    // Instances so that the scriptable object does not modify every instance in the project.
    public IdleSOBase idleSOBaseInstance { get; set; }
    public ChaseSOBase chaseSOBaseInstance { get; set; }
    public AttackSOBase attackSOBaseInstance { get; set; }
    
    #endregion

    protected virtual void Awake()
    {
        idleSOBaseInstance = Instantiate(m_IdleSOBase);
        chaseSOBaseInstance = Instantiate(m_ChaseSOBase);
        attackSOBaseInstance = Instantiate(m_AttackSOBase);

        stateMachine = new StateMachine();

        // Initialize states
        idleState = new IdleState(this, stateMachine);
        chaseState = new ChaseState(this, stateMachine);
        attackState = new AttackState(this, stateMachine);

        pathfollowing = GetComponent<Pathfollowing>();
    }

    protected virtual void Start()
    {
        currentHealth = m_MaxHealth;
        pathfollowing = GetComponent<Pathfollowing>();

        idleSOBaseInstance.Initialize(gameObject, this);
        chaseSOBaseInstance.Initialize(gameObject, this);
        attackSOBaseInstance.Initialize(gameObject, this);

        // Call starting state of the State Machine
        stateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        stateMachine.CurrentState.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.CurrentState.FixedUpdate();
    }


    #region Basic functions

    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public virtual void PerformAttack()
    {
        Debug.LogWarning("PerformAttack not implemented in base Enemy.");
    }

    #endregion

    // TODO: Remove this interface if not used, pathfinding is done in the FSM states.
    #region Movement Functions

    public virtual void Move(Vector2 velocity)
    {
        // TODO: Connect with pathfinding
        // Add check right or left functionality
    }

    public void CheckFacing(Vector2 velocity)
    {
        if (isFacingRight && velocity.x < 0f) Flip();
        else if (!isFacingRight && velocity.x > 0f) Flip();
    }

    public void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    #endregion

    #region Trigger Functions

    public void SetInSensor(bool isInSensor)
    {
        this.isInSensor = isInSensor;
    }

    public void SetWithinAttackRange(bool isWithinAttackRange)
    {
        this.isWithinAttackRange = isWithinAttackRange;
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

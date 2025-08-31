using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittableObject, IDamagable, IMovement, ITrigger, ITransition
{
    #region Damagable variables

    [field: SerializeField] public float m_MaxHealth { get; set; } = 100f;
    public float currentHealth { get; set; }

    #endregion

    #region Movement variables

    public Pathfollowing pathfollowing { get; set; }
    //public bool isFacingRight { get ; set; } = true; // Default sprite is facing right

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
    public Animator animator { get; set; }

    #endregion

    // TODO: FIX THIS MESS. Not sure if this is the right way to do it, will investigate. 
    // This is for variables that have to be different between different objects with the same scriptable object.
    // For instance, two enemies with IdlePatrol will have the same SO. If I want different positions to patrol,
    // I can't set that up in the SO, because then all enemies with that SO will have the same positions, making me 
    // do a different SO for each different position, something completely stupid. For now this is the fix, will
    // think of cleaner ways to do it.
    public Dictionary<string, object> stateContext { get; private set; } = new Dictionary<string, object>();

    public AttackFlagType flagMask;

    public EnemyHit enemyHit; // Assign in inspector

    public AnimatorController animatorController;

    [SerializeField] private ParticleSystem m_DangerParticles; // Assign in inspector

    public virtual void Awake()
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
        animator = GetComponent<Animator>();    
        animatorController = (AnimatorController)animator.runtimeAnimatorController;   
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

    public virtual void ReceiveDamage(float damage, AttackFlagType attackType)
    {
        if ((attackType & flagMask) != 0)
        {
            currentHealth -= damage;

            if (currentHealth <= 0) Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public virtual void PerformAttack()
    {
        //Debug.LogWarning("PerformAttack not implemented in base Enemy.");
        attackSOBaseInstance.PerformAttack();
    }

    public virtual void FinishParry()
    {
        attackSOBaseInstance.FinishParry();
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
        if (pathfollowing.isFacingRight && velocity.x < 0f) Flip();
        else if (!pathfollowing.isFacingRight && velocity.x > 0f) Flip();
    }

    public void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        pathfollowing.isFacingRight = !pathfollowing.isFacingRight;
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

    #region Animation Transitions & Triggers

    public void SetTransitionAnimation(string trigger)
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Chase");
        animator.SetTrigger(trigger);
    }

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

    // TODO: Use these functions to transition between states in each type of enemy.
    #region State Functions 

    public void IdleTransition()
    {
        stateMachine.Transition(idleState);
        SetTransitionAnimation("Idle");
    }

    public void ChaseTransition()
    {
        stateMachine.Transition(chaseState);
        SetTransitionAnimation("Chase");
    }

    public void AttackTransition()
    {
        stateMachine.Transition(attackState);
        SetTransitionAnimation("Attack");
    }

    #endregion

    #region Particles
    private void PlayDangerParticles()
    {
        m_DangerParticles.Play();
    }

    private void StopDangerParticles()
    {
        m_DangerParticles.Stop();
    }
    #endregion
}

using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IMovement, ITrigger
{
    // Damagable variables
    [field: SerializeField] public float m_MaxHealth { get; set; } = 100f;
    public float currentHealth { get; set; }

    // Movement variables
    public Pathfollowing pathfollowing { get; set; }
    public bool isFacingRight { get ; set; } = true; // Default sprite is facing right

    // Trigger variables
    public bool isAggro { get; set; }
    public bool isWithinRange { get; set; }

    // State Machine variables
    public StateMachine stateMachine { get; set; }
    protected Dictionary<System.Type, State> stateRegistry = new();


    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        currentHealth = m_MaxHealth;
        pathfollowing = GetComponent<Pathfollowing>();
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

    #region Distance check Functions
    public void SetAggro(bool isAggro)
    {
        this.isAggro = isAggro;
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

    public virtual T GetState<T>() where T : State
    {
        if (stateRegistry.TryGetValue(typeof(T), out var state))
            return state as T;

        Debug.LogWarning($"State {typeof(T).Name} not found.");
        return null;
    }
}

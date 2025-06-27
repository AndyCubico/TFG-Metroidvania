using UnityEngine;

public class AttackSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void DoEnter() { }
    public virtual void DoExit() { ResetValues(); }
    public virtual void DoUpdate() 
    {
        // TODO: Make smoother
        if (!enemy.isWithinAttackRange)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
            Debug.Log("ATTACK --> CHASE");
        }
    }
    public virtual void DoFixedUpdate() { }
    public virtual void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) { }
    public virtual void ResetValues() { }
}

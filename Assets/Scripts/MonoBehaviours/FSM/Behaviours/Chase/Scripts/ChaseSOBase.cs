using UnityEngine;

public class ChaseSOBase : ScriptableObject
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
        if (enemy.isWithinAttackRange)
        {
            enemy.stateMachine.Transition(enemy.attackState);
            //enemy.SetTransitionAnimation("Attack");
            Debug.Log("CHASE --> ATTACK");
        }
    }
    public virtual void DoFixedUpdate() { }
    public virtual void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) { }
    public virtual void ResetValues() { }
}

using UnityEngine;

public class IdleSOBase : ScriptableObject
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
        if (enemy.isAggro)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
        }
    }
    public virtual void DoFixedUpdate() { }
    public virtual void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) { }
    public virtual void ResetValues() { }

}

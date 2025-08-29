using UnityEngine;

public class AttackSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    public bool isParried;

    public AnimationClip attackClip;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;

        isParried = false;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void DoEnter()
    {
        enemy.SetTransitionAnimation("Attack");
    }

    public virtual void DoExit() { ResetValues(); }
    public virtual void DoUpdate()
    {
        // TODO: Make smoother
        if (!enemy.isWithinAttackRange && !isParried)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
            enemy.SetTransitionAnimation("Chase");
            Debug.Log("ATTACK --> CHASE");
        }
    }
    public virtual void DoFixedUpdate() { }
    public virtual void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) { }
    public virtual void ResetValues() { }
    public virtual void OnParried()
    {
        //Debug.Log("Enemy " + gameObject.name + " has been parried.");
    }

    public virtual void FinishParry()
    {
        //Debug.Log("Parry on enemy " + gameObject.name + " has finished.");
    }

    public virtual void PerformAttack()
    {
        //Debug.Log("PerformAttack not implemented in base Enemy.");
    }
}

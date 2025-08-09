using UnityEngine;

[CreateAssetMenu(fileName = "Static Idle", menuName = "Enemy Logic/Idle/Static Idle")]
public class StaticIdle : IdleSOBase
{
    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void DoEnter()
    {
        base.DoEnter();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        // Fix in case the animator gets stuck with the shooting animation.
        // Not adding transition from shooting to idle so that the animation is not suddenly cut.
        if (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            enemy.SetTransitionAnimation("Idle");
        }
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Attack Slash Combo", menuName = "Enemy Logic/Attack/Attack Slash Combo")]
public class AttackSlashCombo : AttackSOBase
{
    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        base.DoEnter();

        // Not in the base class because some enemies may not have a pathfollowing behaviour.
        enemy.pathfollowing.CancelJump();
        enemy.enemyHit.canBeParried = true;

        enemy.StopDangerParticles();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    public override void OnParried()
    {
        base.OnParried();

        enemy.SetTransitionAnimation("Parried");
        isParried = true;
    }

    public override void FinishParry()
    {
        base.FinishParry();

        isParried = false;
        //enemy.SetTransitionAnimation("Attack");
    }
}

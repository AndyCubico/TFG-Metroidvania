using UnityEngine;

[CreateAssetMenu(fileName = "Danger Attack", menuName = "Enemy Logic/Attack/Danger Attack")]
public class DangerAttack : AttackSOBase
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
        enemy.enemyHit.canBeParried = false;

        enemy.PlayDangerParticles();
    }

    public override void DoExit()
    {
        base.DoExit();

        enemy.StopDangerParticles();
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
}

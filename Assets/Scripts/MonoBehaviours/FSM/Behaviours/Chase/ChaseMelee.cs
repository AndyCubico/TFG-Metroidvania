using UnityEngine;

[CreateAssetMenu(fileName = "Chase Melee", menuName = "Enemy Logic/Chase/Chase Melee")]
public class ChaseMelee : ChaseSOBase
{
    [SerializeField] float m_ChaseSpeed;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        base.DoEnter();
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
        // Set pathfinding route every X seconds
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

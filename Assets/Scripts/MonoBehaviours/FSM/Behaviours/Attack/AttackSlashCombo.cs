using UnityEngine;

[CreateAssetMenu(fileName = "Attack Slash Combo", menuName = "Enemy Logic/Attack/Attack Slash Combo")]
public class AttackSlashCombo : AttackSOBase
{
    // Manage how long it takes to exit the state.
    private float m_ExitTimer;
    [SerializeField] private float m_TimeToExit = 3.0f;
    [SerializeField] private float m_DistanceLimit = 3.0f;

    // Manage how much time it takes between attacks.
    private float m_AttackTimer;
    [SerializeField] private float m_TimeToAttack = 3.0f;

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
        base.DoFixedUpdate();

        // TODO: check with a timer if the player is too far away to attack, return to chase.
        if (m_AttackTimer >= m_TimeToAttack)
        {
            // Attack
        }

        m_AttackTimer += Time.deltaTime;
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

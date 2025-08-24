using UnityEngine;

public class ShieldedEnemy : Enemy
{
    [SerializeField] private bool shieldActive = true;
    [SerializeField] private ShieldHittable shieldObject; // Assign in inspector 

    public override void Awake()
    {
        base.Awake();

        if (shieldObject != null)
        {
            shieldObject.Initialize(this);
        }
    }

    //public override void ReceiveDamage(float damage, AttackFlagType attackType)
    //{
    //    // This is called when the main enemy body is hit
    //    if (shieldActive)
    //    {
    //        // If shield is active, ignore all hits to the body
    //        return;
    //    }

    //    base.ReceiveDamage(damage, attackType);
    //}

    public void ReceiveShieldHit(float damage, AttackFlagType attackType)
    {
        // This is called when the shield collider is hit
        if (!shieldActive)
        {
            base.ReceiveDamage(damage, attackType);
            return;
        }

        if ((attackType & AttackFlagType.HeavyAttack) == 0)
            return;

        BreakShield();
    }

    private void BreakShield()
    {
        shieldActive = false;

        SetTransitionAnimation("ShieldBreak");
    }

    public void DeactivateShield()
    {
        if (shieldObject != null)
        {
            // Allow all attacks to affect this enemy after shield breaks, not optimal but works for now.
            flagMask = AttackFlagType.BasicAttack | AttackFlagType.ImpactHit | AttackFlagType.HeavyAttack | AttackFlagType.SnowAttack | AttackFlagType.Wall;

            shieldObject.gameObject.SetActive(false);
        }
    }
}

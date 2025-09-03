using UnityEngine;

public class StaggerableEnemy : Enemy
{
    [Header("Stagger Settings")]
    [SerializeField] private int heavyHitsToBreak = 3;
    private int currentHeavyHits = 0;

    public override void Awake()
    {
        base.Awake();
        currentHeavyHits = 0;
    }

    public override void ReceiveDamage(float damage, AttackFlagType attackType)
    {
        base.ReceiveDamage(damage, attackType);

        if ((attackType & AttackFlagType.HEAVY_ATTACK) != 0)
        {
            currentHeavyHits++;
            if (currentHeavyHits >= heavyHitsToBreak)
            {
                Stagger();
            }
        }
    }

    private void Stagger()
    {
        currentHeavyHits = 0;
        SetTransitionAnimation("Stagger");
    }
}

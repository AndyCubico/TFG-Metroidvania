using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public PlayerCombatV2 playerCombat;
    public HeavyAttack heavyAttack;

    public void BasicAttackCombatAnimationHasEnded()
    {
        playerCombat.AnimationHasFinished();
    }

    public void StartBasicAttack()
    {
        playerCombat.StartAttacking();
    }

    public void HeavyAttackHasEnded()
    {
        heavyAttack.AnimationHasFinished();
    }

    public void HeavyAttackHit()
    {
        heavyAttack.Hit();
    }
}

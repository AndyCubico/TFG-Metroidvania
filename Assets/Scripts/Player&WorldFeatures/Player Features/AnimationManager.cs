using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public HeavyAttack heavyAttack;

    public void BasicAttackCombatAnimationHasEnded()
    {
        playerCombat.AnimationHasFinished();
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

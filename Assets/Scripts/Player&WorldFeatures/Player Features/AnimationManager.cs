using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public PlayerCombat playerCombat;

    public void BasicAttackCombatAnimationHasEnded()
    {
        playerCombat.AnimationHasFinished();
    }
}

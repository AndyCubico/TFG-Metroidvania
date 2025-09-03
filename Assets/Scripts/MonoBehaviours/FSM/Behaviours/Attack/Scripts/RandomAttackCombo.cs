using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Attack Combo", menuName = "Enemy Logic/Attack/Random Attack Combo")]
public class RandomAttackCombo : AttackSOBase
{
    [SerializeField] private AttackSOBase[] attackOptions;

    private AttackSOBase chosenAttack;

    private AnimatorOverrideController overrideController;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
        foreach (var attack in attackOptions)
            attack.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        ChooseAttack();
    }

    public override void DoUpdate() => chosenAttack?.DoUpdate();
    public override void DoFixedUpdate() => chosenAttack?.DoFixedUpdate();
    public override void DoExit() => chosenAttack?.DoExit();
    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) => chosenAttack?.DoAnimationTrigger(triggerType);
    public override void ResetValues() => chosenAttack?.ResetValues();
    public override void OnParried() => chosenAttack?.OnParried();
    public override void FinishParry() => chosenAttack?.FinishParry();

    public override void PerformAttack()
    {
        if (attackOptions != null && attackOptions.Length > 0)
        {
            ChooseAttack();
        }
    }

    private void ChooseAttack()
    {
        enemy.StopDangerParticles();

        // Pick a random attack that is not the same as the previous one
        int previousIndex = -1;
        if (chosenAttack != null)
        {
            for (int i = 0; i < attackOptions.Length; i++)
            {
                if (attackOptions[i] == chosenAttack)
                {
                    previousIndex = i;
                    break;
                }
            }
        }

        int newIndex;
        if (attackOptions.Length <= 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, attackOptions.Length);
            } while (newIndex == previousIndex);
        }

        chosenAttack = attackOptions[newIndex];

        // Ensure we have an override controller
        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(enemy.animator.runtimeAnimatorController);
            enemy.animator.runtimeAnimatorController = overrideController;
        }

        // Get current overrides
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
        overrideController.GetOverrides(overrides);

        // Now swap the attack clip with the new chosen attack clip
        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key.name == "Attack_Default") // The state of the base attack animation needs this animation by default.
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, chosenAttack.attackClip);
            }
        }

        // Apply
        overrideController.ApplyOverrides(overrides);

        // Trigger the attack logic
        enemy.enemyHit.damage = chosenAttack.damage;
        chosenAttack.DoEnter();
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Attack Combo", menuName = "Enemy Logic/Attack/Random Attack Combo")]
public class RandomAttackCombo : AttackSOBase
{
    [SerializeField] private AttackSOBase[] attackOptions;

    private AttackSOBase chosenAttack;

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

    private AnimatorOverrideController overrideController;
    private AnimationClip previousClip; // track the last clip we replaced

    private void ChooseAttack()
    {
        enemy.StopDangerParticles();

        // Pick a random attack
        chosenAttack = attackOptions[Random.Range(0, attackOptions.Length)];

        // Ensure we have an override controller
        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(enemy.animator.runtimeAnimatorController);
            enemy.animator.runtimeAnimatorController = overrideController;
        }

        // Get current overrides
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
        overrideController.GetOverrides(overrides);

        // If this is the first time, just grab the original clip in the state we want
        if (previousClip == null)
        {
            previousClip = overrides[0].Key;
        }

        // Now swap previousClip with the new chosen attack clip
        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key.name == previousClip.name)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, chosenAttack.attackClip);
            }
        }

        // Apply
        overrideController.ApplyOverrides(overrides);

        // Update previousClip for next round
        previousClip = chosenAttack.attackClip;

        Debug.Log($"Chosen attack: {chosenAttack.name}");

        // Trigger the attack logic
        enemy.enemyHit.damage = chosenAttack.damage;
        chosenAttack.DoEnter();
    }
}
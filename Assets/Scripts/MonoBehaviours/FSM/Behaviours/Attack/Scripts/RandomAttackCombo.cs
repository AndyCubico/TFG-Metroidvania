using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random Attack Combo", menuName = "Enemy Logic/Attack/Random Attack Combo")]
public class RandomAttackCombo : AttackSOBase
{
    [SerializeField] private AttackSOBase[] attackOptions;

    [Tooltip("Probabilities as follows: 50% = 50")]
    [SerializeField] float[] m_AttackProbability;

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

        int newIndex;
        if (attackOptions.Length <= 1)
        {
            newIndex = 0;
        }
        else
        {
            if (m_AttackProbability != null && m_AttackProbability.Length == attackOptions.Length)
            {
                // Weighted random: just pick normally
                newIndex = (int)Choose(m_AttackProbability);
            }
            else
            {
                // Uniform random: avoid repeating previous attack
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

                do
                {
                    newIndex = Random.Range(0, attackOptions.Length);
                } while (newIndex == previousIndex);
            }
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
        attackEnemyHit = chosenAttack.attackEnemyHit;
        attackEnemyHit.damage = chosenAttack.damage;
        chosenAttack.DoEnter();
    }

    // Function to determine which attack to choose https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
    private float Choose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public void SetAttackProbability(float[] probabilities)
    {
        m_AttackProbability = probabilities;
    }
}
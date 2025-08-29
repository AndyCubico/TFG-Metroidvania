using System.Linq;
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
        base.DoEnter();
        chosenAttack = attackOptions[Random.Range(0, attackOptions.Length)];
        chosenAttack.DoEnter();
        var state = enemy.animatorController.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("RandomAttacks")).state;
        enemy.animatorController.SetStateEffectiveMotion(state, chosenAttack.attackClip);
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
        base.PerformAttack();

        if (attackOptions != null && attackOptions.Length > 0)
        {
            ChooseAttack();
        }
    }

    private void ChooseAttack()
    {
            chosenAttack = attackOptions[Random.Range(0, attackOptions.Length)];
            chosenAttack.DoEnter();
            var state = enemy.animatorController.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("RandomAttacks")).state;
            enemy.animatorController.SetStateEffectiveMotion(state, chosenAttack.attackClip);
    }
}
public interface ITrigger
{
    bool isAggro {  get; set; }
    bool isWithinRange { get; set; }

    void SetAggro(bool isTriggered);
    void SetAttackDistance(bool isWithinRange);
}

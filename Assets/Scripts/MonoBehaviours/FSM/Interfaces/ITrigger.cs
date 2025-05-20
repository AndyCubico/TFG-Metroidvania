public interface ITrigger
{
    bool isTriggered {  get; set; }
    bool isWithinRange { get; set; }

    void SetChaseStatus(bool isTriggered);
    void SetAttackDistance(bool isWithinRange);
}

using UnityEngine;

public interface ITrigger
{
    // Vision triggers
    float viewDistance { get; set; }
    float viewAngle { get; set; }
    LayerMask playerLayer { get; set; }
    LayerMask obstacleLayer { get; set; }
    Color fOVColor { get; set; }
    Color hitColor { get; set; }

    // Attack triggers
    bool isAggro { get; set; }
    bool isWithinRange { get; set; }

    void SetAggro(bool isTriggered);
    void SetAttackDistance(bool isWithinRange);
}

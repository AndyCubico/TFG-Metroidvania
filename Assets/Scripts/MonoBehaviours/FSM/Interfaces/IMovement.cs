using UnityEngine;

public interface IMovement
{
    Pathfollowing pathfollowing { get; set; }
    bool isFacingRight { get; set; }

    void Move(Vector2 velocity);

    void CheckFacing(Vector2 velocity);

    void Flip();
}

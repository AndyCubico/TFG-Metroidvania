using UnityEngine;

public interface IMovement
{
    Pathfollowing pathfollowing { get; set; }

    void Move(Vector2 velocity);
}

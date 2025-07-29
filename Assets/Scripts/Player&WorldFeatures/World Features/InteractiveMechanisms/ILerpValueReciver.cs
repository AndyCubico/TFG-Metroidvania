using UnityEngine;

public interface ILerpValueReciver
{
    public float ChangeLerpSource(MonoBehaviour newLerp) { return 0.0f; } // We need to get the past lerp value to not make weird instant movements when changing lerp sources.
}


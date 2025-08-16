//using UnityEditor.ShaderGraph.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class MoveWithLerp : MonoBehaviour, ILerpValueReciver
{
    public List<Transform> positions = new List<Transform>();
    private enum MovementBehaviour 
    {
        A_TO_B, // First transform in the list is orginal position (0) and last one is final position (1). Inbetween are points equitably
        LOOP, // Each point from 0 to 1 travels one pair of the list, then moves from 1 to 0 to the next one.
        STEPS // Each point from 0 to 1 travels one pair of the list, then 0 becomes the last point reached and 1 the next value. 
    }
    [SerializeField] MovementBehaviour behavviour;

    [SerializeField ]int m_CurrentListValue = 0;

    public MonoBehaviour lerpSource; // We cannot use an Interface as a parameter, so we must use a Monobehaviour

    private ILerpValueReturn m_LerpReturner;

    // Control of smooth lerp
    [SerializeField] float m_TargetValue;

    void Awake()
    {
        m_LerpReturner = lerpSource as ILerpValueReturn;
        if (m_LerpReturner == null) {Debug.LogError("Assigned object does not implement ILerpValueReturn"); }
    }
    // Update is called once per frame
    void Update()
    {
        float t = m_LerpReturner.GetCurrentValue();

        switch (behavviour)
        {
            case MovementBehaviour.A_TO_B:
                m_CurrentListValue = (int)((t * (positions.Count - 1)) - 0.0001F);
                m_CurrentListValue = Mathf.Min(positions.Count-2, m_CurrentListValue);
                transform.position = Vector3.Lerp(positions[m_CurrentListValue].position, positions[m_CurrentListValue+1].position, (positions.Count -1) * t - m_CurrentListValue);
                break;
            case MovementBehaviour.LOOP:

                if (m_CurrentListValue % 2 == 0) 
                {
                    transform.position = Vector3.Lerp(positions[m_CurrentListValue].position, positions[(m_CurrentListValue >= positions.Count - 1) ? 0 : m_CurrentListValue + 1].position, t);
                    if (t == 1) 
                    {
                        m_CurrentListValue++;
                    }
                }
                else 
                {
                    transform.position = Vector3.Lerp(positions[(m_CurrentListValue >= positions.Count - 1) ? 0 : m_CurrentListValue +1].position, positions[m_CurrentListValue].position, t);
                    if (t == 0)
                    {
                        m_CurrentListValue++;
                    }
                }

                if (m_CurrentListValue > positions.Count-1) 
                {
                    m_CurrentListValue = 0;
                }
                
                break;
            case MovementBehaviour.STEPS:
                break;
        }
        
    }

    public float ChangeLerpSource(MonoBehaviour newLerp)
    {
        float t = m_LerpReturner.GetCurrentValue();
        lerpSource = newLerp;
        m_LerpReturner = lerpSource as ILerpValueReturn;
        if (m_LerpReturner == null) { Debug.LogError("Assigned object does not implement ILerpValueReturn"); }
        return t;
    }
}

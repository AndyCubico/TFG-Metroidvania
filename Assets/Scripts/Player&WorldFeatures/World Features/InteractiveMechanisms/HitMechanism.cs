using System;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public class HitMechanism : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;

    [Header("Accumulated hit amount")]
    public float currentCharges;
    public float targetCharges;
    [SerializeField] float m_MaxCharges;

    [Header("Countdown")]
    [SerializeField] float m_timeFromFullToZero;
    [SerializeField] bool m_HasTimeResetOnHit;
    [ShowIf("m_HasTimeResetOnHit", true)] public float waitUntilCountdown;
    private float m_CurrentWaitUntilCountdown;
    private bool m_IsCountDown = true;

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if (m_HasTimeResetOnHit) 
            {
                m_CurrentWaitUntilCountdown = waitUntilCountdown;
                m_IsCountDown = false;
            }
                if (currentCharges < m_MaxCharges) 
            {
                currentCharges++;
                currentCharges = Mathf.Min(m_MaxCharges, currentCharges);
            }
        }
    }

    private void Update()
    {
        if(currentCharges > 0 && m_IsCountDown) 
        {
            currentCharges -= Time.deltaTime*m_MaxCharges/(m_timeFromFullToZero);
            currentCharges = Mathf.Max(0, currentCharges);
        }
        else if (m_HasTimeResetOnHit)
        {
            m_CurrentWaitUntilCountdown -= Time.deltaTime;
            if (m_CurrentWaitUntilCountdown <= 0) 
            {
                m_IsCountDown = true;
            }
        }
    }

    public float GetCurrentValue() 
    {
        return currentCharges/ m_MaxCharges;
    }
}

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
    [SerializeField] float m_MaxCharges;

    [Header("Countdown")]
    [SerializeField] float m_timeFromFullToZero;

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if(currentCharges < m_MaxCharges) 
            {
                currentCharges++;
                currentCharges = Mathf.Min(m_MaxCharges, currentCharges);
            }
        }
    }

    private void Update()
    {
        if(currentCharges > 0) 
        {
            currentCharges -= Time.deltaTime*m_MaxCharges/(m_timeFromFullToZero);
            currentCharges = Mathf.Max(0, currentCharges);
        }
    }

    public float GetCurrentValue() 
    {
        return currentCharges/ m_MaxCharges;
    }
}

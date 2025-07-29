using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitLever : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;

    bool m_IsTriggered = false;
    [SerializeField] private float m_UntriggeredValue = 0.0f;
    [SerializeField] private float m_TriggeredValue = 1.0f;
    [SerializeField] private float m_CurrentValue; 
    [SerializeField] float m_timeFromFullToZero;

    [Header("Extra Properties")]
    [SerializeField] bool m_CanBeTurnedOff;
    [SerializeField] bool m_ChangesLerpProvider;
    [ShowIf("m_ChangesLerpProvider", true)] public MonoBehaviour lerpReciver; ILerpValueReciver m_LerpReciver;
    void Awake()
    {
        if (m_ChangesLerpProvider)
        {
            m_LerpReciver = lerpReciver as ILerpValueReciver;
            if (m_LerpReciver == null) { Debug.LogError("Assigned object does not implement ILerpValueReturn"); }
        }
    }
    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if (m_CanBeTurnedOff || !m_IsTriggered)
            {
                m_IsTriggered = !m_IsTriggered;

                if (m_ChangesLerpProvider)
                {
                    m_CurrentValue = m_LerpReciver.ChangeLerpSource(this);
                }

                StartCoroutine(MoveLerp());
            }
        }
    }

    IEnumerator MoveLerp()
    {
        if (m_IsTriggered)
        {
            float sign = Mathf.Sign(m_TriggeredValue - m_CurrentValue);
            while (sign == Mathf.Sign(m_TriggeredValue - m_CurrentValue))
            {
                m_CurrentValue += sign * Time.deltaTime / m_timeFromFullToZero;
                yield return null;
            }
            if (sign != Mathf.Sign(m_TriggeredValue - m_CurrentValue))
            {
                m_CurrentValue = m_TriggeredValue;
            }
        }
        else
        {
            float sign = Mathf.Sign(m_UntriggeredValue - m_CurrentValue);
            while (sign == Mathf.Sign(m_UntriggeredValue - m_CurrentValue))
            {
                m_CurrentValue += sign * Time.deltaTime / m_timeFromFullToZero;
                yield return null;
            }
            if (sign != Mathf.Sign(m_UntriggeredValue - m_CurrentValue))
            {
                m_CurrentValue = m_UntriggeredValue;
            }
        }
            
        
    }

    public float GetCurrentValue() 
    {
        return m_CurrentValue;
    }
}

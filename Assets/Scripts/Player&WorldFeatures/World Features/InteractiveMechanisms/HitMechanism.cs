using System;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public class HitMechanism : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;
    public AttackFlagType snowMask;

    [Header("Accumulated hit amount")]
    public float currentCharges;
    public float targetCharges; // ERIC: TODO, sligth rework the system to make the currentCharges value move slowwly, not in big jumps.
    [SerializeField] float m_MaxCharges;

    [Header("Countdown")]
    [SerializeField] float m_timeFromFullToZero;
    [SerializeField] bool m_HasTimeResetOnHit;
    [ShowIf("m_HasTimeResetOnHit", true)] public float waitUntilCountdown;
    private float m_CurrentWaitUntilCountdown;
    private bool m_IsCountDown = true;
    private bool m_IsFrozen; // Frozen mechanism moves at half speed

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if (m_IsFrozen)
            {
                m_IsFrozen = false;
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
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
        if ((snowMask & flag) != 0)
        {
            m_IsFrozen = true;
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            if (currentCharges < m_MaxCharges)
            {
                currentCharges+= 0.5f;
                currentCharges = Mathf.Min(m_MaxCharges, currentCharges);
            }

            // Hit with the snow spell resets the descend of the mechanism
            if (m_HasTimeResetOnHit)
            {
                m_CurrentWaitUntilCountdown = waitUntilCountdown * 2.0f;
                m_IsCountDown = false;
            }
        }
    }

    private void Update()
    {
        if(currentCharges > 0 && m_IsCountDown) 
        {
            currentCharges -= Time.deltaTime*(m_MaxCharges/m_timeFromFullToZero)*(m_IsFrozen?0.5f:1.0f);
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

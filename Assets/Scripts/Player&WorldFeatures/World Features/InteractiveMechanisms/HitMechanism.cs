using System;
using UnityEngine;

[Serializable]
public class HitMechanism : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;
    public AttackFlagType snowMask;

    [Header("Accumulated hit amount")]
    public float currentCharges;
    public float targetCharges; // Makes the object move smothly no in big jumps
    [SerializeField] float m_MaxCharges;

    [Header("Countdown")]
    [SerializeField] float m_TimeFromFullToZero;
    [SerializeField] bool m_HasTimeResetOnHit;
    [ShowIf("m_HasTimeWaitOnCycle", true)] public float waitUntilCountdown;
    private float m_CurrentWaitUntilCountdown;
    private bool m_IsCountDown = true;
    private bool m_IsFrozen; // Frozen mechanism moves at half speed

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
            if (m_ChangesLerpProvider)
            {
                currentCharges = m_LerpReciver.ChangeLerpSource(this) * m_MaxCharges;
                targetCharges = currentCharges;
            }

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
                if (targetCharges < m_MaxCharges)
                {
                    targetCharges++;
                    targetCharges = Mathf.Min(m_MaxCharges, targetCharges);
                }
            }
        }

        if ((snowMask & flag) != 0)
        {
            m_IsFrozen = true;
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            if (targetCharges < m_MaxCharges)
            {
                targetCharges += 0.5f;
                targetCharges = Mathf.Min(m_MaxCharges, targetCharges);
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
        if(targetCharges > 0 && m_IsCountDown) 
        {
            targetCharges -= Time.deltaTime*(m_MaxCharges/m_TimeFromFullToZero)*(m_IsFrozen?0.5f:1.0f);
            targetCharges = Mathf.Max(0, targetCharges);
            currentCharges = Mathf.Min(targetCharges,currentCharges);
        }
        else if (m_HasTimeResetOnHit)
        {
            m_CurrentWaitUntilCountdown -= Time.deltaTime;
            if (m_CurrentWaitUntilCountdown <= 0) 
            {
                m_IsCountDown = true;
            }
        }

        if(targetCharges>currentCharges)
        {
            currentCharges += Time.deltaTime * (Mathf.Max(1, (targetCharges - currentCharges) * m_TimeFromFullToZero / m_MaxCharges));
            Debug.Log((Mathf.Max(1, (targetCharges - currentCharges) * m_TimeFromFullToZero / m_MaxCharges)));

            if (currentCharges > m_MaxCharges) // If reaches target position the stop time reset to make it feel more that is stoped when reaching maximum 
            {
                m_CurrentWaitUntilCountdown = waitUntilCountdown;
                m_IsCountDown = false;
                currentCharges = Mathf.Min(m_MaxCharges, currentCharges);
            }
           
        }
    }

    public float GetCurrentValue() 
    {
        return currentCharges/ m_MaxCharges;
    }
}

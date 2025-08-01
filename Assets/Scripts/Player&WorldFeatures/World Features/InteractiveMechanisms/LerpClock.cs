using UnityEngine;

public class LerpClock : MonoBehaviour, ILerpValueReturn
{
    [SerializeField] float m_LerpValue;
    [SerializeField] float m_TimeFromFullToZero;
    bool m_IsCountdown = true;
    bool m_IsMovingPositive = true;
    [SerializeField] bool m_HasTimeWaitOnCycle;
    [ShowIf("m_HasTimeWaitOnCycle", true)] public float waitUntilCountdown;
    private float m_CurrentWaitUntilCountdown;

    // Update is called once per frame
    void Update()
    {
        if (m_IsCountdown)
        {
            m_LerpValue += (Time.deltaTime / m_TimeFromFullToZero) * ((m_IsMovingPositive) ? 1.0f : -1.0f);

            if (m_IsMovingPositive && m_LerpValue>= 1.0f)
            {
                m_LerpValue = 1.0f;
                m_IsMovingPositive = false;

                if (m_HasTimeWaitOnCycle)
                {
                    m_IsCountdown = false;
                    m_CurrentWaitUntilCountdown = waitUntilCountdown;
                }
                    
            }
            else if (!m_IsMovingPositive && m_LerpValue <= 0.0f)
            {
                m_LerpValue = 0.0f;
                m_IsMovingPositive = true;

                if (m_HasTimeWaitOnCycle)
                {
                    m_IsCountdown = false;
                    m_CurrentWaitUntilCountdown = waitUntilCountdown;
                }
            }
        }
        else 
        {
            m_CurrentWaitUntilCountdown -= Time.deltaTime;
            if (m_CurrentWaitUntilCountdown<=0) { m_IsCountdown = true;}
        }
    }

    public float GetCurrentValue()
    {
        return m_LerpValue;
    }
}

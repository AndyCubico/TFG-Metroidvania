using UnityEngine;

public class BehaviourManager : MonoBehaviour
{
    private Animator m_Animator;
    private int m_Attack;

    [Tooltip("Probabilities as follows: 50% = 50")]
    [SerializeField] float[] m_AttackProbability;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {

    }

    private void SetAttack()
    {
        m_Attack = (int)Choose(m_AttackProbability);
        m_Animator.SetInteger("ChoosePattern", m_Attack);
    }

    // Function to determine which attack to choose https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
    private float Choose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }
}

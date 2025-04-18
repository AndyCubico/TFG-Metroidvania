using System;
using UnityEngine;

public class RainController : MonoBehaviour
{
    public static RainController Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    //

    [Header("Particles")]
    [SerializeField] private ParticleSystem m_mainParticles;
    [Range(0.0f, 1.0f)]
    public float rainIntensity;
    private float m_maxParticles;

    public event Action<float> eIntentisityChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ParticleSystem.EmissionModule e = m_mainParticles.emission;
        m_maxParticles = e.rateOverTime.constant;
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.EmissionModule e = m_mainParticles.emission;
        e.rateOverTime = rainIntensity * m_maxParticles;

        if (Input.GetKeyDown(KeyCode.K))
        {
            rainIntensity = 0.5f;
            Debug.Log("intensity change " + rainIntensity);

            eIntentisityChange?.Invoke(rainIntensity);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            rainIntensity = 0.15f;
            Debug.Log("intensity change " + rainIntensity);

            eIntentisityChange?.Invoke(rainIntensity);
        }
    }
}

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
    [SerializeField] private ParticleSystem mainParticles;
    [Range(0.0f, 1.0f)]
    public float rainIntensity;
    private float maxParticles;

    public event Action<float> eIntentisityChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ParticleSystem.EmissionModule e = mainParticles.emission;
        maxParticles = e.rateOverTime.constant;
    }

    // Update is called once per frame
    void Update()
    {
        ParticleSystem.EmissionModule e = mainParticles.emission;
        e.rateOverTime = rainIntensity * maxParticles;

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

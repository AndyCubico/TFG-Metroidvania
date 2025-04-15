using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Audio")]
    [SerializeField] private AudioSource lightAudio;
    [SerializeField] private AudioSource mediumAudio;
    //[SerializeField] private AudioSource heavyAudio;
    //public AudioMixerGroup RainSoundAudioMixer;

    [SerializeField] AudioData currentAudioData;
    [SerializeField] AudioData lightAudioData;
    [SerializeField] AudioData mediumAudioData;
    //[SerializeField] AudioData heavyAudioData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ParticleSystem.EmissionModule e = mainParticles.emission;
        maxParticles = e.rateOverTime.constant;

        if (lightAudio)
        {
            lightAudioData = new AudioData(lightAudio);

            if (rainIntensity < 0.45)
            {
                currentAudioData = lightAudioData;
            }
        }

        if (mediumAudio)
        {
            mediumAudioData = new AudioData(mediumAudio);

            if (rainIntensity >= 0.45)
            {
                currentAudioData = mediumAudioData;
            }
        }

        //if (heavyAudio)
        //{
        //    heavyAudioData = new AudioData(heavyAudio);

        //    if (rainIntensity >= 0.45)
        //    {
        //        currentAudioData = heavyAudioData;
        //    }
        //}

        StartCoroutine(currentAudioData.TransitionAudio(0, currentAudioData.defaultVolume));
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
            SetTargetAudio();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            rainIntensity = 0.15f;
            Debug.Log("intensity change " + rainIntensity);
            SetTargetAudio();
        }
    }

    void SetTargetAudio()
    {
        AudioData targetData = currentAudioData;
        if (lightAudio && rainIntensity < 0.45)
        {
            targetData = lightAudioData;
        }
        else if (mediumAudio && rainIntensity >= 0.45)
        {
            targetData = mediumAudioData;
        }

        if (currentAudioData != targetData)
        {
            StartCoroutine(currentAudioData.TransitionAudio(currentAudioData.defaultVolume, 0));
            StartCoroutine(targetData.TransitionAudio(0, currentAudioData.defaultVolume));

            currentAudioData = targetData;
        }
    }
}

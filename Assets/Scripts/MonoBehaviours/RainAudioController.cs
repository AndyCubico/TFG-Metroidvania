using UnityEngine;

public class RainAudioController : MonoBehaviour
{
    [SerializeField] RainController cs_RainController;
    
    [SerializeField] private AudioSource lightAudio;
    [SerializeField] private AudioSource mediumAudio;
    //[SerializeField] private AudioSource heavyAudio;
    //public AudioMixerGroup RainSoundAudioMixer;
    public float transitionTime;

    [SerializeField] AudioData currentAudioData;
    [SerializeField] AudioData lightAudioData;
    [SerializeField] AudioData mediumAudioData;
    //[SerializeField] AudioData heavyAudioData;

    private void OnEnable()
    {
        cs_RainController.eIntentisityChange += SetTargetAudio;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lightAudio)
        {
            lightAudioData = new AudioData(lightAudio, 1, transitionTime);
        }

        if (mediumAudio)
        {
            mediumAudioData = new AudioData(mediumAudio, 1, transitionTime);
        }

        //if (heavyAudio)
        //{
        //    heavyAudioData = new AudioData(heavyAudio, 1, transitionTime);
        //}

        ChangeAudioData(cs_RainController.rainIntensity);
    }

    void ChangeAudioData(float rainIntensity)
    {
        if (rainIntensity < 0.45)
        {
            currentAudioData = lightAudioData;
        }
        else if (rainIntensity >= 0.45)
        {
            currentAudioData = mediumAudioData;
        }
        //    else if (rainIntensity >= 0.45)
        //    {
        //        currentAudioData = heavyAudioData;
        //    }

        StartCoroutine(currentAudioData.TransitionAudio(0, currentAudioData.defaultVolume));
    }

    void SetTargetAudio(float rainIntensity)
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

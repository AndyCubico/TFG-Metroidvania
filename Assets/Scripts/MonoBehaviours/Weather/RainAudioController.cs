using UnityEngine;

public class RainAudioController : MonoBehaviour
{
    [SerializeField] RainController m_cs_RainController;

    [SerializeField] private AudioSource m_lightAudio;
    [SerializeField] private AudioSource m_mediumAudio;
    //[SerializeField] private AudioSource m_heavyAudio;
    //public AudioMixerGroup RainSoundAudioMixer;
    public float transitionTime;

    [SerializeField] private AudioData m_currentAudioData;
    [SerializeField] private AudioData m_lightAudioData;
    [SerializeField] private AudioData m_mediumAudioData;
    //[SerializeField] AudioData m_heavyAudioData;

    private void OnEnable()
    {
        m_cs_RainController.eIntentisityChange += SetTargetAudio;

        m_lightAudioData = new AudioData(m_lightAudio, 1, transitionTime);
        m_mediumAudioData = new AudioData(m_mediumAudio, 1, transitionTime);
        //heavyAudioData = new AudioData(heavyAudio, 1, transitionTime);
       
        ChangeAudioData(m_cs_RainController.rainIntensity);
    }

    private void OnDisable()
    {
        m_cs_RainController.eIntentisityChange -= SetTargetAudio;
    }

    void ChangeAudioData(float rainIntensity)
    {
        if (rainIntensity < 0.45)
        {
            m_currentAudioData = m_lightAudioData;
        }
        else if (rainIntensity >= 0.45)
        {
            m_currentAudioData = m_mediumAudioData;
        }
        //    else if (rainIntensity >= 0.45)
        //    {
        //        currentAudioData = heavyAudioData;
        //    }

        StartCoroutine(m_currentAudioData.TransitionAudio(0, m_currentAudioData.defaultVolume));
    }

    void SetTargetAudio(float rainIntensity)
    {
        AudioData targetData = m_currentAudioData;
        if (m_lightAudio && rainIntensity < 0.45)
        {
            targetData = m_lightAudioData;
        }
        else if (m_mediumAudio && rainIntensity >= 0.45)
        {
            targetData = m_mediumAudioData;
        }

        if (m_currentAudioData != targetData)
        {
            StartCoroutine(m_currentAudioData.TransitionAudio(m_currentAudioData.defaultVolume, 0));
            StartCoroutine(targetData.TransitionAudio(0, m_currentAudioData.defaultVolume));

            m_currentAudioData = targetData;
        }
    }
}

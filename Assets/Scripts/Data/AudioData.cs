using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Wrapper to help with audio functions
/// </summary>
[Serializable]
public class AudioData
{
    public AudioSource audioSource;

    public float defaultVolume;
    public float transitionTime;

    public AudioData(AudioSource audioSc, float defVolume = 1, float tTime = 1)
    {
        audioSource = audioSc;
        defaultVolume = defVolume;
        transitionTime = tTime;

        audioSource.volume = 0;
        audioSource.Stop();
    }

    // Manage transition between two audios
    public IEnumerator TransitionAudio(float defaultVol, float targetVol)
    {
        float percentage = 0;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log($"{audioSource.name} is playing: {audioSource.isPlaying}");
        }

        while (!Mathf.Approximately(audioSource.volume, targetVol))
        {
            audioSource.volume = Mathf.Lerp(defaultVol, targetVol, percentage);
            percentage += Time.deltaTime / transitionTime;

            yield return null;
        }

        if (Mathf.Approximately(audioSource.volume, 0))
        {
            audioSource.Stop();
            Debug.Log($"{audioSource.name} is playing: {audioSource.isPlaying}");
        }

        yield return null;
    }
}

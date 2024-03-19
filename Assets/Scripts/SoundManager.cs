using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance of the LevelManager
    private static SoundManager _instance;

    // Read-only instant of LevelManger to access from other scripts
    public static SoundManager Instance => _instance;

    public AudioClip breathSFX;
    public AudioClip cameraShotSFX;
    public AudioClip noStorageSFX;
    public AudioClip splashSFX;
    public AudioClip cashSFX;
    AudioSource ambientSource;
    AudioSource oneShotSource;

    void Start()
    {
        _instance = this;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        ambientSource = audioSources[0];
        oneShotSource = audioSources[1];
    }

    public void PlayBreathingSFX()
    {
        ambientSource.clip = breathSFX;

        if (!ambientSource.isPlaying)
        {
            ambientSource.Play();
        }
    }

    public void StopBreathingSFX()
    {
        ambientSource.Stop();
    }

    public void PlayCameraSFX()
    {
        oneShotSource.PlayOneShot(cameraShotSFX);
    }

    public void PlayNoStorageSFX()
    {
        oneShotSource.PlayOneShot(noStorageSFX);
    }

    public void PlaySplashSFX()
    {
        oneShotSource.PlayOneShot(splashSFX);
    }

    public void PlayCashSFX()
    {
        oneShotSource.PlayOneShot(cashSFX);
    }
}

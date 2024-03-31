using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance of the LevelManager
    private static SoundManager _instance;

    // Read-only instant of LevelManger to access from other scripts
    public static SoundManager Instance => _instance;

    // Used to stop/start movement noises
    private bool isPlaying = false;

    [Header("Audio Clips")]
    public AudioClip breathSFX;
    public AudioClip cameraShotSFX;
    public AudioClip noStorageSFX;
    public AudioClip splashSFX;
    public AudioClip cashSFX;
    public AudioClip oceanBGMusic;
    public AudioClip surfaceBGMusic;
    public AudioClip oceanBubbleSFX;
    public AudioClip[] sandFootsteps;

    [Header("Audio Sources")]
    AudioSource bgMusicSource;
    AudioSource ambientSource;
    AudioSource oneShotSource;

    void Start()
    {
        
        _instance = this;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        ambientSource = audioSources[0];
        bgMusicSource = audioSources[1];
        oneShotSource = audioSources[2];

    }

    void Update()
    {
        // Starts BG music for surface/ocean
        PlayBGMusic();

      /*  // Plays walking sound effect on surface when input is detected
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) && !isPlaying && !LevelManager.isDiving)
        {
            StartCoroutine(PlayRandomizedAudioCoroutine());
        }
        else if(isPlaying && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            StopSandFootsteps();
        }
        */
    }

    IEnumerator PlayRandomizedAudioCoroutine()
    {
        isPlaying = true;
        float timer = 0f;

        while (timer < 10f)
        {
            int randomIndex = Random.Range(0, sandFootsteps.Length - 1);
            ambientSource.clip = sandFootsteps[randomIndex];
            
            // Adjusts pitch to speed up the audio clip to make it sound nicer
            ambientSource.pitch = 1.2f;
            ambientSource.volume = 0.1f;

            ambientSource.Play();

            yield return new WaitForSecondsRealtime(ambientSource.clip.length / 1.2f);
            timer += ambientSource.clip.length / 1.2f;
        }

        // Reset pitch to normal/bool to false to reset source
        ambientSource.pitch = 1f;
        isPlaying = false;
    }

    public void StopSandFootsteps()
    {
        ambientSource.Stop();
    }

    public void PlayBGMusic()
    {
        if(LevelManager.isDiving)
        {
            bgMusicSource.clip = oceanBGMusic;
            bgMusicSource.volume = 0.3f;
        }
        else
        {
            bgMusicSource.clip = surfaceBGMusic;
            bgMusicSource.volume = 0.05f;
        }

        if(!bgMusicSource.isPlaying)
        {
            bgMusicSource.Play();
        }
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

    public void PlayOceanBubbleSFX()
    {
        oneShotSource.PlayOneShot(oceanBubbleSFX);
    }
}

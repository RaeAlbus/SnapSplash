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

    private Coroutine bubbleCoroutine;

    void Update()
    {
        // Starts BG music for surface/ocean
        PlayBGMusic();

        // Plays bubble sounds repeatedly only if underwater
        if(LevelManager.isDiving)
        {
            if(bubbleCoroutine == null)
            {
                bubbleCoroutine = StartCoroutine(PlayOceanBubbleSFXCoroutine());
            }
        }
        else
        {
            // If not diving --> stop playing bubbles / start walking sound
            if(bubbleCoroutine != null)
            {
                StopCoroutine(bubbleCoroutine);
                bubbleCoroutine = null;
            }

            // Plays walking sound effect on surface when input is detected
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) && !isPlaying && !LevelManager.isDiving)
            {
                StartCoroutine(PlaySandFootStepsCoroutine());
            }
            else if(isPlaying && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            {
                StopSandFootsteps();
            }
        }
    }

    IEnumerator PlaySandFootStepsCoroutine()
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

            // Wait for the clip to finish playing
            yield return new WaitForSecondsRealtime(ambientSource.clip.length / 1.2f);

            // Update the timer based on the actual time elapsed
            timer += Time.deltaTime;
        }

        // Reset pitch to normal/bool to false to reset source
        ambientSource.pitch = 1f;
        isPlaying = false;
    }

    // Plays bubble sounds every 8-12 seconds

    IEnumerator PlayOceanBubbleSFXCoroutine()
    {
        while(LevelManager.isDiving)
        {
            PlayOceanBubbleSFX();
            yield return new WaitForSeconds(Random.Range(8f, 10f));
        }
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

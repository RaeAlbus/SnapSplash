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
    public AudioClip sandFootstepsSFX;

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
            // If not diving --> stop playing bubbles
            if(bubbleCoroutine != null)
            {
                StopCoroutine(bubbleCoroutine);
                bubbleCoroutine = null;
            }

            
            // If walking --> play footsteps sound
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) && !ShopKeeperBehavior.inShop)
            {
                ambientSource.enabled = true;
            }
            else
            {
                ambientSource.enabled = false;
            }
        }
    }

    //------------------------------------BG MUSIC SOUNDS----------------------------

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

    //------------------------------------AMBIENT SOUNDS----------------------------

    public void PlayWalkingSFX()
    {
        ambientSource.volume = 0.05f;
        ambientSource.clip = sandFootstepsSFX;
    }

    public void StopWalkingSFX()
    {
        ambientSource.clip = sandFootstepsSFX;
        ambientSource.volume = 0.25f;
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

    //------------------------------------ONESHOT SOUNDS----------------------------

    // Plays bubble sounds every 8-12 seconds
    IEnumerator PlayOceanBubbleSFXCoroutine()
    {
        while(LevelManager.isDiving)
        {
            PlayOceanBubbleSFX();
            yield return new WaitForSeconds(Random.Range(8f, 10f));
        }
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

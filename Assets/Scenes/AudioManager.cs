using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // Аудиофайлы
    public AudioClip backgroundMusic;
    public AudioClip bossMusic;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip victorySound;

    // Источники звука
    private AudioSource backgroundAudioSource;
    private AudioSource bossAudioSource;
    private AudioSource sfxAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = backgroundMusic;
        backgroundAudioSource.volume = 0.3f; // Уменьшаем громкость фоновой музыки

        bossAudioSource = gameObject.AddComponent<AudioSource>();
        bossAudioSource.loop = true;
        bossAudioSource.clip = bossMusic;
        bossAudioSource.volume = 0.6f;

        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = 0.8f;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (!backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
            Debug.Log("Playing background music.");
        }
    }

    public void PlayBossMusic()
    {
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
            Debug.Log("Stopped background music.");
        }
        if (!bossAudioSource.isPlaying)
        {
            bossAudioSource.Play();
            Debug.Log("Playing boss music.");
        }
    }

    public void StopBossMusic()
    {
        if (bossAudioSource.isPlaying)
        {
            bossAudioSource.Stop();
            Debug.Log("Stopped boss music.");
        }
        // Запускаем корутину для задержки перед воспроизведением фоновой музыки
        StartCoroutine(DelayedPlayBackgroundMusic(2f));
    }

    private IEnumerator DelayedPlayBackgroundMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBackgroundMusic();
    }

    public void StopAllMusic()
    {
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
            Debug.Log("Stopped background music.");
        }
        if (bossAudioSource.isPlaying)
        {
            bossAudioSource.Stop();
            Debug.Log("Stopped boss music.");
        }
    }

    public void PlayCorrectAnswerSound()
    {
        sfxAudioSource.PlayOneShot(correctAnswerSound);
        Debug.Log("Playing correct answer sound.");
    }

    public void PlayWrongAnswerSound()
    {
        sfxAudioSource.PlayOneShot(wrongAnswerSound);
        Debug.Log("Playing wrong answer sound.");
    }

    public void PlayVictorySound()
    {
        sfxAudioSource.PlayOneShot(victorySound);
        Debug.Log("Playing victory sound.");
    }
}
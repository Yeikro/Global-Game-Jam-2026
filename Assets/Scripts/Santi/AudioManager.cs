using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource dialogSource;
    public AudioSource sfxSource;

    [Header("Música")]
    public AudioClip backgroundMusic;

    [Header("Ambiente")]
    public AudioClip ambientLoop;

    [Header("Diálogos")]
    public List<AudioClip> dialogClips = new List<AudioClip>();
    public float dialogMinDelay = 8f;
    public float dialogMaxDelay = 15f;

    [Header("SFX")]
    public AudioClip rayShot;
    public AudioClip dirtAppear;
    public AudioClip uiSelect;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMusic();
        PlayAmbient();
        StartCoroutine(DialogLoop());
    }

    // =========================
    // MÚSICA
    // =========================

    public void PlayMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // =========================
    // AMBIENTE
    // =========================

    public void PlayAmbient()
    {
        if (ambientLoop == null) return;

        ambientSource.clip = ambientLoop;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    // =========================
    // DIÁLOGOS RANDOM
    // =========================

    IEnumerator DialogLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(dialogMinDelay, dialogMaxDelay);
            yield return new WaitForSeconds(waitTime);

            if (dialogClips.Count == 0 || dialogSource.isPlaying)
                continue;

            AudioClip clip = dialogClips[Random.Range(0, dialogClips.Count)];
            dialogSource.PlayOneShot(clip);
        }
    }

    // =========================
    // SFX
    // =========================

    public void PlayRay()
    {
        if (rayShot != null)
            sfxSource.PlayOneShot(rayShot);
    }

    public void PlayDirtAppear()
    {
        if (dirtAppear != null)
            sfxSource.PlayOneShot(dirtAppear);
    }

    public void PlayUISelect()
    {
        if (uiSelect != null)
            sfxSource.PlayOneShot(uiSelect);
    }

    public void PlayCustomSFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
}


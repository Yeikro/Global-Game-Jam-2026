using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 2f;
    public float timeBeforeEndToFade = 2f;

    [Header("Scene")]
    public string nextSceneName;

    private bool isFading = false;

    void Start()
    {
        // Asegura que el fade esté visible al inicio
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        // Fade IN (de negro a transparente)
        StartCoroutine(Fade(1f, 0f, 1f));

        videoPlayer.Play();
    }

    void Update()
    {
        if (!videoPlayer.isPlaying || isFading)
            return;

        double remainingTime = videoPlayer.length - videoPlayer.time;

        if (remainingTime <= timeBeforeEndToFade)
        {
            isFading = true;
            StartCoroutine(FadeAndChangeScene());
        }
    }

    IEnumerator FadeAndChangeScene()
    {
        // Fade OUT (de transparente a negro)
        yield return Fade(0f, 1f, fadeDuration);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}

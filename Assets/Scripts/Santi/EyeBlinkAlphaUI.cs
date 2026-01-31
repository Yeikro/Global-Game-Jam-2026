using UnityEngine;
using UnityEngine.UI;

public class EyeBlinkAlphaUI : MonoBehaviour
{
    [Header("Face Solver")]
    public ARKitFaceSolver faceSolver;

    [Header("UI Images")]
    public Image leftEyeImage;
    public Image rightEyeImage;

    [Header("Parpadeo")]
    [Range(0f, 1f)] public float blinkThreshold = 0.7f;
    [Range(0.05f, 0.3f)] public float alphaStep = 0.15f;

    [Header("Espíritu / Visión")]
    public SpiritAI spiritAI;
    public Transform player;

    [Tooltip("Tiempo mínimo para cegarte")]
    public float minBlindTime = 1.2f;

    [Tooltip("Tiempo máximo para cegarte")]
    public float maxBlindTime = 3.0f;

    bool leftWasBlinking;
    bool rightWasBlinking;

    float blindTimer;
    float nextBlindTime;
    bool visionTaken;

    void Start()
    {
        RollBlindTime();
    }

    void Update()
    {
        if (faceSolver == null) return;

        HandleBlink(faceSolver.eyeBlink_L, leftEyeImage, ref leftWasBlinking);
        HandleBlink(faceSolver.eyeBlink_R, rightEyeImage, ref rightWasBlinking);

        HandleSpiritVision();
    }

    // =========================
    // PARPADEO → BAJA ALPHA
    // =========================
    void HandleBlink(float blinkValue, Image img, ref bool wasBlinking)
    {
        if (img == null) return;

        bool isBlinking = blinkValue >= blinkThreshold;

        // Detectar inicio del parpadeo
        if (isBlinking && !wasBlinking)
        {
            Color c = img.color;
            c.a = Mathf.Clamp01(c.a - alphaStep);
            img.color = c;

            Debug.Log($"{img.name} parpadeo → alpha {c.a}");
        }

        wasBlinking = isBlinking;
    }

    // =========================
    // ESPÍRITU TE VE → ALPHA = 1
    // =========================
    void HandleSpiritVision()
    {
        if (spiritAI == null || player == null) return;

        float dist = Vector3.Distance(player.position, spiritAI.transform.position);

        bool insideVision =
            dist <= spiritAI.radioVision ||
            spiritAI.estadoActual == SpiritAI.Estado.Huida;

        if (!insideVision)
        {
            blindTimer = 0f;
            visionTaken = false;
            RollBlindTime();
            return;
        }

        blindTimer += Time.deltaTime;

        if (!visionTaken && blindTimer >= nextBlindTime)
        {
            BlindInstant();
            visionTaken = true;

            Debug.Log("El espíritu te quitó la visión");
        }
    }

    void BlindInstant()
    {
        SetAlpha(leftEyeImage, 1f);
        SetAlpha(rightEyeImage, 1f);
    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    void RollBlindTime()
    {
        nextBlindTime = Random.Range(minBlindTime, maxBlindTime);
    }

    // =========================
    // RESET TOTAL
    // =========================
    public void ResetVision()
    {
        SetAlpha(leftEyeImage, 0f);
        SetAlpha(rightEyeImage, 0f);
        blindTimer = 0f;
        visionTaken = false;
        RollBlindTime();
    }
}
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

    [Header("Espíritus / Visión")]
    public Transform player;
    public SpiritAI[] spirits;   // 👈 VARIOS SPIRIT AI

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
        // Si no los asignas a mano, los busca solos
        if (spirits == null || spirits.Length == 0)
            spirits = FindObjectsOfType<SpiritAI>();

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

        if (isBlinking && !wasBlinking)
        {
            Color c = img.color;
            c.a = Mathf.Clamp01(c.a - alphaStep);
            img.color = c;
        }

        wasBlinking = isBlinking;
    }

    // =========================
    // CUALQUIER ESPÍRITU TE VE
    // =========================
    void HandleSpiritVision()
    {
        if (player == null || spirits == null) return;

        bool insideAnyVision = false;

        foreach (SpiritAI spirit in spirits)
        {
            if (spirit == null) continue;

            float dist = Vector3.Distance(player.position, spirit.transform.position);

            if (dist <= spirit.radioVision ||
                spirit.estadoActual == SpiritAI.Estado.Huida)
            {
                insideAnyVision = true;
                break; // 👈 con uno basta
            }
        }

        if (!insideAnyVision)
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

            Debug.Log("Un espíritu te quitó la visión");
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
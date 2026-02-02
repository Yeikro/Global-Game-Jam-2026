using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MouthChargeController : MonoBehaviour
{
    [Header("Referencia Face Solver")]
    public ARKitFaceSolver faceSolver;

    [Header("Referencia Player")]
    public PlayerRBController playerController;

    [Header("Sliders")]
    public Slider sliderPrimary;
    public Slider sliderSecondary;

    [Header("Configuración")]
    [Range(0f, 1f)] public float jawThreshold = 0.9f;
    public UnityEvent eventoDisparo;
    public UnityEvent eventoCansado;
    public UnityEvent eventoCargando;
    public UnityEvent eventoEstuneado;

    public float primaryChargeTime = 1.2f;
    public float secondaryChargeTime = 1.0f;

    public float primaryDecaySpeed = 8f;     // rápido
    public float secondaryDecaySpeed = 2.5f; // lento
    public float finalDecayCharge = 2.5f;

    bool primaryCompleted;
    bool secondaryCompleted;
    bool finalDischarge;

    void Update()
    {
        if (faceSolver == null) return;

        bool mouthOpen = faceSolver.jawOpen >= jawThreshold;

        if (finalDischarge)
        {
            HandleFinalDischarge();
            return;
        }

        if (mouthOpen)
        {
            HandleCharge();
        }
        else
        {
            HandleEarlyReset();
        }

        Debug.Log(eventoEstuneado);

    }

    // =========================
    // CARGA
    // =========================
    void HandleCharge()
    {
        // ---- CARGA PRIMARIA ----
        if (!primaryCompleted)
        {
            sliderPrimary.value += Time.deltaTime / primaryChargeTime;
            eventoCargando.Invoke();

            if (sliderPrimary.value >= 1f)
            {
                sliderPrimary.value = 1f;
                primaryCompleted = true;
                Debug.Log("EVENTO 1: Carga primaria completa");
                eventoDisparo.Invoke();
            }

            sliderSecondary.value += Time.deltaTime / secondaryChargeTime;

            if (sliderSecondary.value >= 1f)
            {
                sliderSecondary.value = 1f;
                secondaryCompleted = true;
                finalDischarge = true;

                Debug.Log("EVENTO 2: Carga secundaria completa");
                eventoCansado.Invoke();
            }

            return;
        }

        // ---- CARGA SECUNDARIA ----
        if (!secondaryCompleted)
        {
            sliderSecondary.value += Time.deltaTime / secondaryChargeTime;

            if (sliderSecondary.value >= 1f)
            {
                sliderSecondary.value = 1f;
                secondaryCompleted = true;
                finalDischarge = true;

                Debug.Log("EVENTO 2: Carga secundaria completa");
                eventoEstuneado.Invoke();
                eventoCansado.Invoke();
            }
        }
    }

    // =========================
    // RESET SI SE CIERRA ANTES
    // =========================
    void HandleEarlyReset()
    {
        sliderPrimary.value = Mathf.MoveTowards(
            sliderPrimary.value, 0f, Time.deltaTime * primaryDecaySpeed);

        sliderSecondary.value = Mathf.MoveTowards(
            sliderSecondary.value, 0f, Time.deltaTime * secondaryDecaySpeed);

        primaryCompleted = false;
        secondaryCompleted = false;
        eventoCansado.Invoke();
    }

    // =========================
    // DESCARGA FINAL
    // =========================
    void HandleFinalDischarge()
    {
        sliderPrimary.value = Mathf.MoveTowards(
            sliderPrimary.value, 0f, Time.deltaTime * primaryDecaySpeed);

        sliderSecondary.value = Mathf.MoveTowards(
            sliderSecondary.value, 0f, Time.deltaTime * finalDecayCharge);

        if (sliderPrimary.value <= 0f && sliderSecondary.value <= 0f)
        {
            ResetCharge();
            playerController.blockNormalMovement = false;
        }
    }

    // =========================
    // RESET TOTAL
    // =========================
    void ResetCharge()
    {
        sliderPrimary.value = 0f;
        sliderSecondary.value = 0f;

        primaryCompleted = false;
        secondaryCompleted = false;
        finalDischarge = false;
    }
}
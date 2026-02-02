using UnityEngine;

public class ARKitFaceSolver : MonoBehaviour
{
    //public FaceDataReceiver receiver;

    [Header("Calibración")]
    public bool calibrarNeutral;

    [HideInInspector] public float neutralJaw;
    [HideInInspector] public float neutralMouthWidth;
    [HideInInspector] public float neutralBrowY;
    [HideInInspector] public float eyeOpenNeutralL;
    [HideInInspector] public float eyeOpenNeutralR;
    [HideInInspector] public float neutralYaw;


    [Header("Coeficientes ARKit (0–1)")]
    public float jawOpen;
    public float mouthSmile_L;
    public float mouthSmile_R;
    public float browInnerUp;
    public float eyeBlink_L;
    public float eyeBlink_R;

    [Header("Rotación cabeza")]
    [Range(-1f, 1f)] public float headYaw;
    [Range(0.01f, 1f)] public float smoothYaw = 0.12f;

    [Header("Suavizado")]
    [Range(0.01f, 1f)] public float smoothJaw = 0.15f;
    [Range(0.01f, 1f)] public float smoothSmile = 0.12f;
    [Range(0.01f, 1f)] public float smoothBrow = 0.12f;
    [Range(0.01f, 1f)] public float smoothBlink = 0.25f;

    Vector3 faceCenter;
    Quaternion faceRotation;

    void Start()
    {
        if (PlayerPrefs.HasKey("neutralJaw"))
        {
            neutralJaw = PlayerPrefs.GetFloat("neutralJaw");
            neutralMouthWidth = PlayerPrefs.GetFloat("neutralMouthWidth");
            neutralBrowY = PlayerPrefs.GetFloat("neutralBrowY");
            eyeOpenNeutralL = PlayerPrefs.GetFloat("eyeOpenNeutralL");
            eyeOpenNeutralR = PlayerPrefs.GetFloat("eyeOpenNeutralR");
            neutralYaw = PlayerPrefs.GetFloat("neutralYaw");
        }
    }

    void Update()
    {
        FaceDataReceiver receiver = FaceDataReceiver.instance;

        if (receiver == null) return;

        var boca = receiver.bocaNormalizada;
        var ojoL = receiver.ojoIzquierdoNormalizado;
        var ojoR = receiver.ojoDerechoNormalizado;
        var cejaL = receiver.cejaIzquierdaNormalizada;
        var cejaR = receiver.cejaDerechaNormalizada;

        if (boca.Length < 4 || ojoL.Length < 4 || ojoR.Length < 4)
            return;

        ConstruirEspacioFacial(ojoL, ojoR);

        // ========= YAW (HEAD TURN) =========
        Vector3 centroL = Promedio(ojoL);
        Vector3 centroR = Promedio(ojoR);

        Vector3 dir = (centroR - centroL).normalized;
        float yawRaw = dir.z; // positivo = gira derecha

        float yawTarget = Mathf.Clamp(yawRaw - neutralYaw, -1f, 1f);
        headYaw = Mathf.Lerp(headYaw, yawTarget, smoothYaw);

        // ========= JAW =========
        float jawRaw = Mathf.Abs(Local(boca[3]).y - Local(boca[2]).y);
        float jawTarget = Mathf.Clamp01((jawRaw - neutralJaw) / 0.025f);
        jawOpen = Mathf.Lerp(jawOpen, jawTarget, smoothJaw);

        // ========= SMILE =========
        float width = Local(boca[1]).x - Local(boca[0]).x;
        float smileRaw = Mathf.Clamp01((width - neutralMouthWidth) / 0.03f);
        mouthSmile_L = Mathf.Lerp(mouthSmile_L, smileRaw, smoothSmile);
        mouthSmile_R = mouthSmile_L;

        // ========= EYES =========
        float eyeOpenL = EyeOpenLocal(ojoL);
        float eyeOpenR = EyeOpenLocal(ojoR);

        float blinkTargetL =
            (eyeOpenNeutralL - eyeOpenL) / (eyeOpenNeutralL * 0.7f);
        float blinkTargetR =
            (eyeOpenNeutralR - eyeOpenR) / (eyeOpenNeutralR * 0.7f);

        blinkTargetL = Mathf.Clamp01(blinkTargetL - 0.05f);
        blinkTargetR = Mathf.Clamp01(blinkTargetR - 0.05f);

        eyeBlink_L = Mathf.Lerp(eyeBlink_L, blinkTargetL, smoothBlink);
        eyeBlink_R = Mathf.Lerp(eyeBlink_R, blinkTargetR, smoothBlink);

        // ========= BROW =========
        float browRaw =
            (Local(cejaL[0]).y + Local(cejaR[0]).y) * 0.5f;

        float browTarget =
            Mathf.Clamp01((browRaw - neutralBrowY) / 0.02f);

        browInnerUp =
            Mathf.Lerp(browInnerUp, browTarget, smoothBrow);

        // ========= CALIBRACIÓN =========
        if (calibrarNeutral)
        {
            neutralJaw = Mathf.Lerp(neutralJaw, jawRaw, 0.1f);
            neutralMouthWidth = Mathf.Lerp(neutralMouthWidth, width, 0.1f);
            neutralBrowY = Mathf.Lerp(neutralBrowY, browRaw, 0.1f);
            eyeOpenNeutralL = Mathf.Lerp(eyeOpenNeutralL, eyeOpenL, 0.1f);
            eyeOpenNeutralR = Mathf.Lerp(eyeOpenNeutralR, eyeOpenR, 0.1f);
            neutralYaw = Mathf.Lerp(neutralYaw, yawRaw, 0.1f);
        }
    }

    // ==========================
    // ESPACIO FACIAL
    // ==========================
    void ConstruirEspacioFacial(Vector3[] ojoL, Vector3[] ojoR)
    {
        Vector3 centroL = Promedio(ojoL);
        Vector3 centroR = Promedio(ojoR);

        faceCenter = (centroL + centroR) * 0.5f;

        Vector3 right = (centroR - centroL).normalized;
        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.Cross(right, up);

        faceRotation = Quaternion.LookRotation(forward, up);
    }

    Vector3 Local(Vector3 p)
    {
        return Quaternion.Inverse(faceRotation) * (p - faceCenter);
    }

    // ==========================
    // UTILS
    // ==========================
    float EyeOpenLocal(Vector3[] ojo)
    {
        float upper = Local(ojo[2]).y;
        float lower = Local(ojo[3]).y;
        float v = Mathf.Abs(upper - lower);

        float left = Local(ojo[0]).x;
        float right = Local(ojo[1]).x;
        float h = Mathf.Abs(right - left);

        if (h < 0.0001f) return 0f;
        return v / h;
    }

    Vector3 Promedio(Vector3[] pts)
    {
        Vector3 sum = Vector3.zero;
        foreach (var p in pts) sum += p;
        return sum / pts.Length;
    }

    public void Calibrar()
    {
        calibrarNeutral = true;
    }
}



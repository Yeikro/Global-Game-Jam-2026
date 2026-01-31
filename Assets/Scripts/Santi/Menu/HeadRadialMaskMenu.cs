using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadRadialMaskMenu : MonoBehaviour
{
    [Header("Slots visibles")]
    public RectTransform slotLeft;
    public RectTransform slotCenter;
    public RectTransform slotRight;

    [Header("RawImages")]
    public RawImage imgLeft;
    public RawImage imgCenter;
    public RawImage imgRight;

    [Header("RenderTextures (UI)")]
    public List<RenderTexture> sections = new List<RenderTexture>();

    [Header("Objetos 3D en escena (mismo orden que sections)")]
    public List<GameObject> sceneObjects = new List<GameObject>();

    [Header("Animación")]
    public float moveDistance = 180f;
    public float scaleCenter = 1.25f;
    public float scaleSide = 0.9f;
    public float duration = 0.35f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    int currentIndex = 0;
    bool animando = false;

    Vector2 posLeft, posCenter, posRight;

    void Start()
    {
        posLeft = slotLeft.anchoredPosition;
        posCenter = slotCenter.anchoredPosition;
        posRight = slotRight.anchoredPosition;

        ActualizarTextures();
        ActivarObjetoCentral();
        ResetVisual();
    }

    // =========================
    // API
    // =========================

    public void GirarDerecha()
    {
        if (animando || sections.Count < 3) return;
        StartCoroutine(AnimarDerecha());
    }

    public void GirarIzquierda()
    {
        if (animando || sections.Count < 3) return;
        StartCoroutine(AnimarIzquierda());
    }

    // =========================
    // ANIMACIONES
    // =========================

    IEnumerator AnimarDerecha()
    {
        animando = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float e = ease.Evaluate(t);

            slotCenter.anchoredPosition = Vector2.Lerp(posCenter, posRight, e);
            slotRight.anchoredPosition = Vector2.Lerp(posRight, posRight + Vector2.right * moveDistance, e);
            slotLeft.anchoredPosition = Vector2.Lerp(posLeft - Vector2.right * moveDistance, posLeft, e);

            slotCenter.localScale = Vector3.Lerp(Vector3.one * scaleCenter, Vector3.one * scaleSide, e);
            slotLeft.localScale = Vector3.Lerp(Vector3.one * scaleSide, Vector3.one * scaleCenter, e);
            slotRight.localScale = Vector3.Lerp(Vector3.one * scaleSide, Vector3.zero, e);

            yield return null;
        }

        currentIndex = Mod(currentIndex - 1, sections.Count);

        ResetSlots();
        ActualizarTextures();
        ActivarObjetoCentral();
        ResetVisual();

        animando = false;
    }

    IEnumerator AnimarIzquierda()
    {
        animando = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float e = ease.Evaluate(t);

            slotCenter.anchoredPosition = Vector2.Lerp(posCenter, posLeft, e);
            slotLeft.anchoredPosition = Vector2.Lerp(posLeft, posLeft - Vector2.right * moveDistance, e);
            slotRight.anchoredPosition = Vector2.Lerp(posRight + Vector2.right * moveDistance, posRight, e);

            slotCenter.localScale = Vector3.Lerp(Vector3.one * scaleCenter, Vector3.one * scaleSide, e);
            slotRight.localScale = Vector3.Lerp(Vector3.one * scaleSide, Vector3.one * scaleCenter, e);
            slotLeft.localScale = Vector3.Lerp(Vector3.one * scaleSide, Vector3.zero, e);

            yield return null;
        }

        currentIndex = Mod(currentIndex + 1, sections.Count);

        ResetSlots();
        ActualizarTextures();
        ActivarObjetoCentral();
        ResetVisual();

        animando = false;
    }

    // =========================
    // VISUAL UI
    // =========================

    void ActualizarTextures()
    {
        int left = Mod(currentIndex - 1, sections.Count);
        int right = Mod(currentIndex + 1, sections.Count);

        imgLeft.texture = sections[left];
        imgCenter.texture = sections[currentIndex];
        imgRight.texture = sections[right];
    }

    void ResetVisual()
    {
        slotLeft.anchoredPosition = posLeft;
        slotCenter.anchoredPosition = posCenter;
        slotRight.anchoredPosition = posRight;

        slotCenter.localScale = Vector3.one * scaleCenter;
        slotLeft.localScale = Vector3.one * scaleSide;
        slotRight.localScale = Vector3.one * scaleSide;

        imgCenter.color = Color.white;
        imgLeft.color = Color.white * 0.7f;
        imgRight.color = Color.white * 0.7f;

        slotCenter.SetAsLastSibling();
    }

    void ResetSlots()
    {
        slotLeft.anchoredPosition = posLeft;
        slotCenter.anchoredPosition = posCenter;
        slotRight.anchoredPosition = posRight;
    }

    // =========================
    // OBJETOS 3D
    // =========================

    void ActivarObjetoCentral()
    {
        if (sceneObjects.Count != sections.Count)
        {
            Debug.LogWarning("sceneObjects y sections no tienen el mismo tamaño");
            return;
        }

        for (int i = 0; i < sceneObjects.Count; i++)
        {
            sceneObjects[i].SetActive(i == currentIndex);
        }
    }

    int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }
}
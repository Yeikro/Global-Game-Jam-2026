using UnityEngine;

public class YawMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public ARKitFaceSolver faceSolver;
    public HeadRadialMaskMenu menu;

    [Header("Yaw settings")]
    public float yawThreshold = 0.3f;
    public float resetThreshold = 0.15f;

    bool bloqueado = false;

    void Update()
    {
        if (faceSolver == null || menu == null)
            return;

        float yaw = faceSolver.headYaw;

        // 🔓 Desbloqueo cuando vuelve al centro
        if (bloqueado && Mathf.Abs(yaw) < resetThreshold)
        {
            bloqueado = false;
        }

        if (bloqueado)
            return;

        // 👉 Girar a la derecha
        if (yaw > yawThreshold)
        {
            menu.GirarDerecha();
            bloqueado = true;
        }
        // 👈 Girar a la izquierda
        else if (yaw < -yawThreshold)
        {
            menu.GirarIzquierda();
            bloqueado = true;
        }
    }
}


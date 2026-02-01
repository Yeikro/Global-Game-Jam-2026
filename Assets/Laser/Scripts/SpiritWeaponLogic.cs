using UnityEngine;

public class SpiritWeaponLogic : MonoBehaviour
{
    [Header("Conexión con el Sistema")]
    public SpiritWeaponController controladorArma;

    // NOTA: Ya no necesitamos Camera ni Plane porque el disparo es Forward

    public void AlternarDisparo()
    {
        if (controladorArma == null) return;

        if (controladorArma.EstaDisparando)
        {
            controladorArma.DetenerDisparo();
        }
        else
        {
            controladorArma.IniciarDisparo();
        }
    }

    // CAMBIO: Ya no pide Vector2 de pantalla. Solo procesa el frame actual.
    public void ProcesarFrame()
    {
        if (controladorArma == null || !controladorArma.EstaDisparando) return;

        // Simplemente le decimos al arma que actualice su estado (física, movimiento, etc.)
        // El arma usará su propio transform.forward
        controladorArma.ActualizarDisparo();
    }

    void Update()
    {
        ProcesarFrame();
    }
}
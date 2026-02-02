using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritWeaponInputAdapter : MonoBehaviour
{
    [Header("Referencias")]
    public SpiritWeaponLogic logicaArma;

    [Header("Configuración New Input System")]
    public InputActionProperty accionDisparar;

    void OnEnable()
    {
        if (accionDisparar.action != null)
        {
            accionDisparar.action.Enable();
            accionDisparar.action.performed += AlPresionarBoton;
        }
    }

    void OnDisable()
    {
        if (accionDisparar.action != null)
        {
            accionDisparar.action.performed -= AlPresionarBoton;
            accionDisparar.action.Disable();
        }
    }

    private void AlPresionarBoton(InputAction.CallbackContext context)
    {
        logicaArma.AlternarDisparo();
    }
}
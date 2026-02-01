using UnityEngine;

public class SpiritWeaponInput : MonoBehaviour
{
    [Header("Referencias")]
    public SpiritWeaponController controladorArma;
    public Camera camaraPrincipal;

    // Usamos un plano matemático para calcular la posición del mouse en el mundo 3D
    private Plane _planoPiso;

    void Start()
    {
        // Si no asignaste cámara, busca la principal
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        // Creamos un plano infinito a la altura de este objeto (piso virtual)
        _planoPiso = new Plane(Vector3.up, transform.position);
    }

    void Update()
    {
        if (controladorArma == null) return;

        // 1. Calcular dónde está apuntando el jugador
        Vector3 puntoObjetivo = CalcularPuntoMouse();

        // 2. Detectar Click Inicial (Down)
        if (Input.GetMouseButtonDown(0))
        {
            controladorArma.IniciarDisparo();
        }

        // 3. Detectar Mantener Click (Hold)
        if (Input.GetMouseButton(0))
        {
            // Le enviamos continuamente la posición actualizada
            //controladorArma.ActualizarDisparo(puntoObjetivo);
        }

        // 4. Detectar Soltar Click (Up)
        if (Input.GetMouseButtonUp(0))
        {
            controladorArma.DetenerDisparo();
        }
    }

    Vector3 CalcularPuntoMouse()
    {
        // Actualizamos la altura del plano por si el personaje saltó o subió escaleras
        _planoPiso.SetNormalAndPosition(Vector3.up, transform.position);

        Ray rayo = camaraPrincipal.ScreenPointToRay(Input.mousePosition);
        float distancia;

        if (_planoPiso.Raycast(rayo, out distancia))
        {
            return rayo.GetPoint(distancia);
        }

        // Fallback por si acaso clicamos al infinito
        return transform.position + transform.forward * 10f;
    }
}
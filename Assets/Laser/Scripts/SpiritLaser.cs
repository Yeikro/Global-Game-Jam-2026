using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpiritLaser : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [Header("Configuración Visual")]
    public int segmentos = 50;
    public float alturaOnda = 0.5f;
    public float velocidadOnda = 4.0f;
    public float velocidadRetraccion = 20.0f; // Velocidad al recogerse

    // Estado interno
    private bool _estaSiendoControlado = true;
    private Vector3 _posicionCola;
    private Vector3 _posicionCabeza;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = segmentos;
        _lineRenderer.useWorldSpace = true; // Vital para moverse libremente
    }

    // --- FASE 1: MIENTRAS EL JUGADOR DISPARA ---
    // Este método lo llama SpiritWeaponController frame a frame
    public void ActualizarDesdeArma(Vector3 inicio, Vector3 fin)
    {
        if (!_estaSiendoControlado) return;

        _posicionCola = inicio;
        _posicionCabeza = fin;

        DibujarOnda();
    }

    // --- FASE 2: CUANDO EL JUGADOR SUELTA ---
    // El arma llama a esto para desconectarse
    public void SoltarRayo()
    {
        _estaSiendoControlado = false;
    }

    void Update()
    {
        // Si el arma todavía controla este láser, no hacemos nada aquí
        if (_estaSiendoControlado) return;

        // --- LÓGICA DE AUTO-RETRACCIÓN (Independiente) ---
        // La cola viaja hacia la cabeza
        _posicionCola = Vector3.MoveTowards(_posicionCola, _posicionCabeza, velocidadRetraccion * Time.deltaTime);

        // Actualizamos el dibujo
        DibujarOnda();

        // Chequeo de Muerte: Si la cola llega a la cabeza, destruir
        if (Vector3.Distance(_posicionCola, _posicionCabeza) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void DibujarOnda()
    {
        for (int i = 0; i < segmentos; i++)
        {
            float t = (float)i / (segmentos - 1);
            Vector3 posBase = Vector3.Lerp(_posicionCola, _posicionCabeza, t);

            // Matemática de la onda suave
            float onda = Mathf.Sin(t * Mathf.PI * 2 + Time.time * velocidadOnda);
            float anclaje = Mathf.Sin(t * Mathf.PI); // Fija los extremos
            Vector3 deformacion = Vector3.up * onda * alturaOnda * anclaje;

            _lineRenderer.SetPosition(i, posBase + deformacion);
        }
    }
}
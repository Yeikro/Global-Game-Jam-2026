using UnityEngine;

public class SpiritWeaponController : MonoBehaviour
{
    [Header("Configuración General")]
    public SpiritLaser prefabRayo;
    public Transform puntoSalida;
    public LayerMask capasColision;
    public string tagObjetivo = "Espiritu";

    [Header("Parámetros Disparo")]
    public float velocidadExtension = 15f;
    public float alcanceMaximo = 20f;

    [Header("Parámetros de Captura")]
    public float velocidadArrastre = 5.0f;
    public float rangoDeCaptura = 2.0f;

    public bool EstaDisparando => _rayoActivo != null;

    // --- ESTADO INTERNO ---
    private SpiritLaser _rayoActivo;

    // CAMBIO: Ya no guardamos la posición de la punta para moverla,
    // guardamos qué tan largo es el rayo en este momento.
    private float _longitudActual = 0f;

    // Estado Captura
    private bool _estaArrastrando = false;
    private GameObject _objetoCapturado;
    private GameObject _anclaArrastre;

    // --- API PÚBLICA ---

    public void IniciarDisparo()
    {
        if (_rayoActivo != null) DetenerDisparo();

        _rayoActivo = Instantiate(prefabRayo, Vector3.zero, Quaternion.identity);

        // Reseteamos la longitud a 0 para que empiece a crecer desde la mano
        _longitudActual = 0f;
        _estaArrastrando = false;
    }

    public void DetenerDisparo()
    {
        if (_estaArrastrando && _objetoCapturado != null)
        {
            _objetoCapturado.transform.SetParent(null);
        }

        if (_anclaArrastre != null) Destroy(_anclaArrastre);

        _estaArrastrando = false;
        _objetoCapturado = null;

        if (_rayoActivo != null)
        {
            _rayoActivo.SoltarRayo();
            _rayoActivo = null;
        }
    }

    public void ActualizarDisparo()
    {
        if (_rayoActivo == null) return;

        if (_estaArrastrando)
        {
            ProcesarArrastre();
        }
        else
        {
            ProcesarExtensionRigida();
        }
    }

    // --- LÓGICA INTERNA ---

    void ProcesarExtensionRigida()
    {
        // 1. Calcular la distancia máxima posible en ESTE frame
        // (Depende de si estoy mirando a una pared o al aire)
        float distanciaObjetivo = CalcularDistanciaColision();

        // 2. Aumentar la longitud del rayo suavemente (Extensión progresiva)
        // Usamos MoveTowards pero con valores float (escalares), no vectores.
        _longitudActual = Mathf.MoveTowards(_longitudActual, distanciaObjetivo, velocidadExtension * Time.deltaTime);

        // 3. Reconstruir la posición de la punta basada en la rotación ACTUAL de la mano
        // Esto garantiza que si giras la mano, el rayo gira instantáneamente.
        Vector3 puntaVisual = puntoSalida.position + (puntoSalida.forward * _longitudActual);

        // 4. Actualizar visuales
        _rayoActivo.ActualizarDesdeArma(puntoSalida.position, puntaVisual);

        // 5. Verificar captura (Si la longitud ya casi tocó el objetivo)
        // Usamos una pequeña tolerancia (0.1f)
        if (Mathf.Abs(_longitudActual - distanciaObjetivo) < 0.1f)
        {
            VerificarCaptura(puntaVisual);
        }
    }

    float CalcularDistanciaColision()
    {
        // Lanzamos rayo invisible para saber hasta dónde PODEMOS llegar hoy
        if (Physics.Raycast(puntoSalida.position, puntoSalida.forward, out RaycastHit hit, alcanceMaximo, capasColision))
        {
            return hit.distance; // La pared/enemigo limita el rayo
        }
        else
        {
            return alcanceMaximo; // No hay obstáculo, extendemos al máximo
        }
    }

    void VerificarCaptura(Vector3 posicionPunta)
    {
        // Confirmar qué objeto tocamos exactamente
        if (Physics.Raycast(puntoSalida.position, puntoSalida.forward, out RaycastHit hit, _longitudActual + 0.1f, capasColision))
        {
            if (hit.collider.CompareTag(tagObjetivo))
            {
                IniciarCaptura(hit.collider.gameObject, hit.point);
            }
        }
    }

    void IniciarCaptura(GameObject enemigo, Vector3 puntoImpacto)
    {
        _estaArrastrando = true;
        _objetoCapturado = enemigo;
        _anclaArrastre = new GameObject("Ancla_Captura_Temp");
        _anclaArrastre.transform.position = puntoImpacto;
        _objetoCapturado.transform.SetParent(_anclaArrastre.transform);
    }

    void ProcesarArrastre()
    {
        if (_anclaArrastre == null || _objetoCapturado == null)
        {
            DetenerDisparo();
            return;
        }

        // NOTA: Al arrastrar, el rayo SÍ se desconecta del "Forward" rígido
        // porque ahora está físicamente conectado al objeto que arrastras.

        _anclaArrastre.transform.position = Vector3.MoveTowards(
            _anclaArrastre.transform.position,
            puntoSalida.position,
            velocidadArrastre * Time.deltaTime
        );

        Vector3 puntaVisual = _anclaArrastre.transform.position;
        _rayoActivo.ActualizarDesdeArma(puntoSalida.position, puntaVisual);

        if (Vector3.Distance(_anclaArrastre.transform.position, puntoSalida.position) <= rangoDeCaptura)
        {
            EjecutarAbsorcion();
        }
    }

    void EjecutarAbsorcion()
    {
        Debug.Log("¡Absorbido!");
        if (_objetoCapturado != null) Destroy(_objetoCapturado);
        if (_anclaArrastre != null) Destroy(_anclaArrastre);
        DetenerDisparo();
    }
}
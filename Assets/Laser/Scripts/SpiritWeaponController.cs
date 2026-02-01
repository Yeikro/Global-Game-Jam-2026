using UnityEngine;

public class SpiritWeaponController : MonoBehaviour
{
    [Header("Configuración General")]
    public SpiritLaser prefabRayo;
    public Transform puntoSalida;
    public Vector3 puntoFinal;
    public LayerMask capasColision;
    public string tagObjetivo = "Spirit";

    [Header("Parámetros Disparo")]
    public float velocidadExtension = 15f;
    public float alcanceMaximo = 20f;

    [Header("Parámetros de Captura")]
    public float velocidadArrastre = 5.0f;
    public float rangoDeCaptura = 2.0f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // --- ESTADO INTERNO ---
    private SpiritLaser _rayoActivo;
    private Vector3 _puntaActual;
    private Vector3 _destinoDeseado;
    private Vector3 _destinoCalculado;

    // Estado Captura
    private bool _estaArrastrando = false;
    private GameObject _objetoCapturado;
    private GameObject _anclaArrastre;


    public Camera camaraPrincipal;

    // Usamos un plano matemático para calcular la posición del mouse en el mundo 3D
    private Plane _planoPiso;
    // NOTA: Se eliminó _posicionOriginalEnemigo porque ya no reseteamos

    // --- API PÚBLICA ---

    private void Start()
    {
        // Si no asignaste cámara, busca la principal
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;

        // Creamos un plano infinito a la altura de este objeto (piso virtual)
        _planoPiso = new Plane(Vector3.up, transform.position);
    }

    public void IniciarDisparo()
    {
        if (_rayoActivo != null) DetenerDisparo();

        _rayoActivo = Instantiate(prefabRayo, Vector3.zero, Quaternion.identity);
        _puntaActual = puntoSalida.position;
        _estaArrastrando = false;
    }

    public void ActualizarDisparo()
    {
        if (_rayoActivo == null) return;

        _destinoDeseado = CalcularPuntoMouse();

        if (_estaArrastrando)
        {
            ProcesarArrastre();
        }
        else
        {
            ProcesarExtensionYBusqueda();
        }
        Debug.Log("se está actualizando");
    }

    public void DetenerDisparo()
    {
        // 1. SOLTAR AL ENEMIGO (CAMBIO SOLICITADO)
        if (_estaArrastrando && _objetoCapturado != null)
        {
            // Solo le quitamos el padre. Se quedará donde esté en este momento.
            _objetoCapturado.transform.SetParent(null);

            Debug.Log("Captura cancelada: Enemigo soltado");
        }

        // 2. Limpieza de objetos temporales
        if (_anclaArrastre != null) Destroy(_anclaArrastre);

        _estaArrastrando = false;
        _objetoCapturado = null;

        // 3. Liberar el rayo visual
        if (_rayoActivo != null)
        {
            _rayoActivo.SoltarRayo();
            _rayoActivo = null;
        }
    }

    // --- LÓGICA INTERNA ---

    void ProcesarExtensionYBusqueda()
    {
        CalcularColisionesFisicas();

        _puntaActual = Vector3.MoveTowards(_puntaActual, _destinoCalculado, velocidadExtension * Time.deltaTime);

        _rayoActivo.ActualizarDesdeArma(puntoSalida.position, _puntaActual);

        // Verificamos captura solo si la punta llegó al destino
        if (Vector3.Distance(_puntaActual, _destinoCalculado) < 0.1f)
        {
            VerificarCaptura();
        }
    }

    void CalcularColisionesFisicas()
    {
        Vector3 direccion = (_destinoDeseado - puntoSalida.position).normalized;
        float distanciaMouse = Vector3.Distance(puntoSalida.position, _destinoDeseado);
        float distanciaMaxima = Mathf.Min(distanciaMouse, alcanceMaximo);

        if (Physics.Raycast(puntoSalida.position, direccion, out RaycastHit hit, distanciaMaxima, capasColision))
        {
            _destinoCalculado = hit.point;
        }
        else
        {
            if (distanciaMouse > alcanceMaximo)
                _destinoCalculado = puntoSalida.position + (direccion * alcanceMaximo);
            else
                _destinoCalculado = _destinoDeseado;
        }
    }

    void VerificarCaptura()
    {
        Vector3 direccion = (_destinoCalculado - puntoSalida.position).normalized;
        float distancia = Vector3.Distance(puntoSalida.position, _destinoCalculado);

        if (Physics.Raycast(puntoSalida.position, direccion, out RaycastHit hit, distancia + 0.1f, capasColision))
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
        // Ya no guardamos la posición original

        // Crear Ancla
        _anclaArrastre = new GameObject("Ancla_Captura_Temp");
        _anclaArrastre.transform.position = puntoImpacto;

        // Emparentar
        _objetoCapturado.transform.SetParent(_anclaArrastre.transform);
    }

    void ProcesarArrastre()
    {
        if (_anclaArrastre == null || _objetoCapturado == null)
        {
            DetenerDisparo();
            return;
        }

        // Mover el ancla hacia la mano
        _anclaArrastre.transform.position = Vector3.MoveTowards(
            _anclaArrastre.transform.position,
            puntoSalida.position,
            velocidadArrastre * Time.deltaTime
        );

        // El rayo apunta al ancla
        _puntaActual = _anclaArrastre.transform.position;
        _rayoActivo.ActualizarDesdeArma(puntoSalida.position, _puntaActual);

        // Absorción completa
        if (Vector3.Distance(_anclaArrastre.transform.position, puntoSalida.position) <= rangoDeCaptura)
        {
            EjecutarAbsorcion();
        }
    }

    void EjecutarAbsorcion()
    {
        Debug.Log("<color=green>¡OBJETIVO ABSORBIDO!</color>");

        if (_objetoCapturado != null)
        {
            _objetoCapturado.GetComponent<SpiritAI>().estadoActual = SpiritAI.Estado.Huida;
            _objetoCapturado.SetActive(false);
            _objetoCapturado.transform.SetParent(PlayerRBController.instance.transform);
        }
        if (_anclaArrastre != null) Destroy(_anclaArrastre);

        DetenerDisparo();
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

    private void Update()
    {
        ActualizarDisparo();
    }

    // --- DEBUG ---
    void OnDrawGizmos()
    {
        if (!mostrarGizmos || puntoSalida == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(puntoSalida.position, rangoDeCaptura);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(puntoSalida.position, alcanceMaximo);
    }
}
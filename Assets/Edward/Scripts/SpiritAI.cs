using UnityEngine;
using UnityEngine.AI;

public class SpiritAI : MonoBehaviour
{
    // Definicion de Estados
    public enum Estado { Patrulla, Alerta, Huida, Aturdido }

    [Header("Configuracion General")]
    public Estado estadoActual = Estado.Patrulla;
    public string tagJugador = "Player";
    public Transform objetivoJugador;

    [Header("Sentidos")]
    public float radioVision = 10f;
    public float radioSeguridad = 15f;
    [Range(0, 360)] public float anguloVision = 90f;
    public float alturaOjos = 1.0f;

    [Header("Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadHuida = 6f;
    public float radioPatrulla = 15f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    // Variables internas
    private NavMeshAgent agente;
    private float tiempoEsperaPatrulla = 0f;
    private bool jugadorVisible = false;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();

        GameObject objJugador = GameObject.FindGameObjectWithTag(tagJugador);
        if (objJugador != null)
        {
            objetivoJugador = objJugador.transform;
        }
    }

    void Update()
    {
        if (objetivoJugador == null) return;

        // 1. Actualizar Sentidos
        jugadorVisible = DetectarJugador();

        // 2. Maquina de Estados
        switch (estadoActual)
        {
            case Estado.Patrulla:
                LogicaPatrulla();
                break;
            case Estado.Alerta:
                LogicaAlerta();
                break;
            case Estado.Huida:
                LogicaHuida();
                break;
            case Estado.Aturdido:
                LogicaAturdido();
                break;
        }
    }

    // --- COMPORTAMIENTOS ---

    void LogicaAturdido()
    {
        agente.ResetPath();
    }

    void LogicaPatrulla()
    {
        agente.speed = velocidadPatrulla;

        if (jugadorVisible)
        {
            estadoActual = Estado.Huida;
            return;
        }

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            if (tiempoEsperaPatrulla <= 0f)
            {
                Vector3 puntoAleatorio = ObtenerPuntoNavMesh(transform.position, radioPatrulla);
                agente.SetDestination(puntoAleatorio);
                tiempoEsperaPatrulla = 2f;
            }
            else
            {
                tiempoEsperaPatrulla -= Time.deltaTime;
            }
        }
    }

    void LogicaAlerta()
    {
        if (jugadorVisible)
        {
            estadoActual = Estado.Huida;
        }
        else
        {
            estadoActual = Estado.Patrulla;
        }
    }

    void LogicaHuida()
    {
        agente.speed = velocidadHuida;

        float distancia = Vector3.Distance(transform.position, objetivoJugador.position);

        if (distancia > radioSeguridad)
        {
            estadoActual = Estado.Patrulla;
            return;
        }

        Vector3 direccionHuida = transform.position - objetivoJugador.position;
        Vector3 destinoHuida = transform.position + direccionHuida.normalized * 5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destinoHuida, out hit, 2.0f, NavMesh.AllAreas))
        {
            agente.SetDestination(hit.position);
        }
    }

    // --- SISTEMA SENSORIAL ---

    bool DetectarJugador()
    {
        float distancia = Vector3.Distance(transform.position, objetivoJugador.position);
        if (distancia > radioVision) return false;

        Vector3 direccionJugador = (objetivoJugador.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, direccionJugador) < anguloVision / 2)
        {
            Vector3 origenOjos = transform.position + Vector3.up * alturaOjos;

            if (Physics.Raycast(origenOjos, direccionJugador, out RaycastHit hit, radioVision))
            {
                if (hit.collider.CompareTag(tagJugador))
                {
                    Debug.DrawLine(origenOjos, hit.point, Color.red);
                    return true;
                }
            }
        }
        return false;
    }

    // --- UTILIDADES Y DEBUG ---

    Vector3 ObtenerPuntoNavMesh(Vector3 origen, float radio)
    {
        Vector3 direccionAleatoria = Random.insideUnitSphere * radio;
        direccionAleatoria += origen;
        NavMeshHit hit;
        NavMesh.SamplePosition(direccionAleatoria, out hit, radio, 1);
        return hit.position;
    }

    void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;

        Gizmos.color = jugadorVisible ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioVision);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radioSeguridad);

        // 3. Cono de Vision (Azul)
        Vector3 anguloA = DireccionDesdeAngulo(-anguloVision / 2, false);
        Vector3 anguloB = DireccionDesdeAngulo(anguloVision / 2, false);
        Vector3 origenOjos = transform.position + Vector3.up * alturaOjos;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origenOjos, origenOjos + anguloA * radioVision);
        Gizmos.DrawLine(origenOjos, origenOjos + anguloB * radioVision);
    }

    public Vector3 DireccionDesdeAngulo(float anguloEnGrados, bool anguloGlobal)
    {
        if (!anguloGlobal)
        {
            anguloEnGrados += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(anguloEnGrados * Mathf.Deg2Rad), 0, Mathf.Cos(anguloEnGrados * Mathf.Deg2Rad));
    }
}
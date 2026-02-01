using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerRBController : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionProperty moveAction;     // Vector2
    public InputActionProperty mousePosAction; // Vector2

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 6f;
    public bool blockNormalMovement;
    public Vector3 desiredVel;

    [Header("Refs")]
    public Camera mainCamera;
    public Rigidbody rb;
    public AnimaSola anim;

    Vector3 targetPos;

    void Awake()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (!mainCamera) mainCamera = Camera.main;
    }

    void OnEnable() { moveAction.action.Enable(); mousePosAction.action.Enable(); }
    void OnDisable() { moveAction.action.Disable(); mousePosAction.action.Disable(); }

    void Update()
    {
        // calcular velocidad deseada en Update (input)
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 dir = new Vector3(input.x, 0, input.y).normalized;



        desiredVel = dir * moveSpeed;
        if (!blockNormalMovement)
        {
            PointTowardsMouse(); // solo rota (no mueve)

            if (dir.magnitude > 0.1f)
                anim.CambiarACaminar();
            else
                anim.CambiarAIdle();
        }
        else
        {
            Fall(); // rota según velocidad actual al caer
        }
    }

    void FixedUpdate()
    {
        // aplicar movimiento suave en físicas
        if (!blockNormalMovement)
        {
            targetPos = rb.position + desiredVel * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
        }
    }

    void PointTowardsMouse()
    {
        if (!mainCamera) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // En vez de un plano fijo Y=0, mejor raycast al ground real:
        if (Physics.Raycast(ray, out var hit, 200f, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
                rb.MoveRotation(Quaternion.LookRotation(lookDir));
        }
        // Si quieres mantener el plano Y=0:
        // Plane ground = new Plane(Vector3.up, Vector3.zero);
        // if (ground.Raycast(ray, out float enter)) { ... }
    }

    void Fall()
    {
        Vector3 velocity = rb.velocity;

        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(velocity.normalized);

            Quaternion tiltRotation = lookRotation * Quaternion.Euler(90, 0, 0);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                tiltRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    public void Tired()
    {
        blockNormalMovement = true;
        rb.velocity = Vector3.zero;
        anim.CambiarACansado();
        Debug.Log("Player is tired and cannot move.");
    }
}

using FishNet.Object;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Solo el dueño del objeto (jugador) puede controlarlo
        if (!base.IsOwner)
            return;

        // Obtener entrada del usuario
        float moveInput = Input.GetAxis("Vertical"); // W/S o flechas arriba/abajo
        float turnInput = Input.GetAxis("Horizontal"); // A/D o flechas izquierda/derecha

        // Movimiento y rotación local
        Vector3 moveDirection = transform.forward * moveInput * speed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotación del coche
        float turnAmount = turnInput * turnSpeed * Time.deltaTime;
        Quaternion turnOffset = Quaternion.Euler(0, turnAmount, 0);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}



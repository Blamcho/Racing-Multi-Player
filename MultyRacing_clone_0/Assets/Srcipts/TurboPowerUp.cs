using FishNet.Object;
using UnityEngine;

public class TurboPowerUp : NetworkBehaviour
{
    public float turboMultiplier = 2f; // Multiplicador de velocidad del turbo
    public float turboDuration = 10f; // Duración del turbo en segundos
    private bool isTurboActive = false;

    public CarController carController;
    private float originalSpeed;

    private void Start()
    {
        carController = GetComponent<CarController>();
        originalSpeed = carController.speed;
    }

    
    [ServerRpc]
    public void ActivateTurboServerRpc()
    {
        if (!isTurboActive)
        {
            // Activa el turbo en el servidor
            isTurboActive = true;
            carController.speed *= turboMultiplier; // Aumenta la velocidad del coche
            // Propaga el turbo a todos los clientes
            ActivateTurboObserversRpc();
            // Inicia la cuenta regresiva para desactivar el turbo
            Invoke(nameof(DeactivateTurbo), turboDuration);
        }
    }

    
    [ObserversRpc]
    private void ActivateTurboObserversRpc()
    {
        carController.speed *= turboMultiplier; // Sincroniza la velocidad en los clientes
    }

    // Desactiva el turbo
    private void DeactivateTurbo()
    {
        isTurboActive = false;
        carController.speed = originalSpeed; // Restaura la velocidad original
        DeactivateTurboObserversRpc();
    }

    // Propaga la desactivación del turbo a todos los clientes
    [ObserversRpc]
    private void DeactivateTurboObserversRpc()
    {
        carController.speed = originalSpeed; 
    }
}

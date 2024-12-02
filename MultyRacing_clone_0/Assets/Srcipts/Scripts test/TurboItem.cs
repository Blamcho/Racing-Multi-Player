using System.Collections;
using UnityEngine;

public class TurboItem : MonoBehaviour
{
    public float turboMultiplier = 2f; // Multiplicador de la velocidad del turbo
    public float turboDuration = 5f;   // Duración del turbo en segundos

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            CarControllers carController = other.GetComponent<CarControllers>();
            if (carController != null)
            {
                StartCoroutine(ActivateTurbo(carController));
            }

            
            gameObject.SetActive(false); 
        }
    }

    private IEnumerator ActivateTurbo(CarControllers carController)
    {
       
        float originalMotorForce = carController.motorForce;

        
        carController.motorForce *= turboMultiplier;

        
        yield return new WaitForSeconds(turboDuration);

        
        carController.motorForce = originalMotorForce;
    }
}

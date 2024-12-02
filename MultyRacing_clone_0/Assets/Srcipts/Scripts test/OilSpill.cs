using System.Collections;
using UnityEngine;

public class OilSpill : MonoBehaviour
{
    public float slipDuration = 3f;    
    public float reducedControlFactor = 0.3f; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            CarControllers carController = other.GetComponent<CarControllers>();
            if (carController != null)
            {
                StartCoroutine(ApplySlipEffect(carController));
            }
        }
    }

    private IEnumerator ApplySlipEffect(CarControllers carController)
    {
        
        float originalMotorForce = carController.motorForce;
        carController.motorForce *= reducedControlFactor;
        foreach (var wheel in new WheelCollider[] { carController.frontLeftWheel, carController.frontRightWheel, carController.rearLeftWheel, carController.rearRightWheel })
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.stiffness = 0.5f; 
            wheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.stiffness = 0.5f; 
            wheel.sidewaysFriction = sidewaysFriction;
        }

        yield return new WaitForSeconds(slipDuration);

        
        carController.motorForce = originalMotorForce;
        foreach (var wheel in new WheelCollider[] { carController.frontLeftWheel, carController.frontRightWheel, carController.rearLeftWheel, carController.rearRightWheel })
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.stiffness = 1f; 
            wheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.stiffness = 1f; 
            wheel.sidewaysFriction = sidewaysFriction;
        }
    }
}

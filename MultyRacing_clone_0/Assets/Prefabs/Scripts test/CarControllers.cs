using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllers : MonoBehaviour
{
    public float motorForce = 1500f;
    public float steeringAngle = 30f;
    public float brakeForce = 3000f;
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;
    public float rolloverThreshold = 0.5f; // Umbral de vuelco en el eje Y
    public float correctionDelay = 2f;     // Tiempo en segundos antes de corregir la posición

    private float horizontalInput;
    private float verticalInput;
    private float steeringInput;
    private float currentBrakeForce;
    private bool isBraking;
    private float rolloverTimer;

    private void Update()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        CheckRollover();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A/D keys for steering
        verticalInput = Input.GetAxis("Vertical");     // W/S keys for forward/reverse
        isBraking = Input.GetKey(KeyCode.Space);       // Space for braking
    }

    private void HandleMotor()
    {
        frontLeftWheel.motorTorque = verticalInput * motorForce;
        frontRightWheel.motorTorque = verticalInput * motorForce;
        rearLeftWheel.motorTorque = verticalInput * motorForce;
        rearRightWheel.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheel.brakeTorque = currentBrakeForce;
        frontRightWheel.brakeTorque = currentBrakeForce;
        rearLeftWheel.brakeTorque = currentBrakeForce;
        rearRightWheel.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        steeringInput = horizontalInput * steeringAngle;
        frontLeftWheel.steerAngle = steeringInput;
        frontRightWheel.steerAngle = steeringInput;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftTransform);
        UpdateSingleWheel(frontRightWheel, frontRightTransform);
        UpdateSingleWheel(rearLeftWheel, rearLeftTransform);
        UpdateSingleWheel(rearRightWheel, rearRightTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void CheckRollover()
    {
        // Verifica si el coche está inclinado más allá del umbral
        if (Mathf.Abs(transform.up.y) < rolloverThreshold)
        {
            rolloverTimer += Time.deltaTime;

            // Corrige la posición si el tiempo de vuelco es mayor al tiempo de corrección
            if (rolloverTimer >= correctionDelay)
            {
                CorrectCarPosition();
            }
        }
        else
        {
            // Reinicia el temporizador si el coche no está volcado
            rolloverTimer = 0f;
        }
    }

    private void CorrectCarPosition()
    {
        // Restaura el coche a su posición correcta
        transform.position += Vector3.up; // Eleva el coche un poco para evitar que se atasque en el suelo
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        rolloverTimer = 0f; // Reinicia el temporizador después de corregir
    }
}

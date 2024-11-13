using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class CarControllers : NetworkBehaviour
{
    public float motorForce = 1500f;
    public float steeringAngle = 30f;
    public float brakeForce = 3000f;
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;
    public float correctionDelay = 3f;     // Tiempo en segundos antes de corregir la posición

    private float horizontalInput;
    private float verticalInput;
    private float steeringInput;
    private float currentBrakeForce;
    private bool isBraking;
    private float rolloverTimer;

    private void Update()
    {
       
        if (!IsOwner) return;

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
        if (transform.up.y < 0)
        {
            rolloverTimer += Time.deltaTime;

            if (rolloverTimer >= correctionDelay)
            {
                CorrectCarPosition();
            }
        }
        else
        {
            rolloverTimer = 0f;
        }
    }

    private void CorrectCarPosition()
    {
        transform.position += Vector3.up;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        rolloverTimer = 0f;
    }
}

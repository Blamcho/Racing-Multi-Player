using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using FishNet.Object;

[System.Serializable]
public class CarConfig
{
    public float motorForce;
    public float steeringAngle;
    public float brakeForce;
}

public class CarControllers : NetworkBehaviour
{
    public float motorForce = 1500f;
    public float steeringAngle = 30f;
    public float brakeForce = 3000f;
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;
    public float correctionDelay = 3f;

    private float horizontalInput;
    private float verticalInput;
    private float steeringInput;
    private float currentBrakeForce;
    private bool isBraking;
    private float rolloverTimer;
    private float upsideDownTimer;

    private Vector3 syncedPosition;
    private Quaternion syncedRotation;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            StartCoroutine(FetchCarConfigFromAPI());
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            UpdateWheels();
            CheckRollover();
            CheckUpsideDown();
            SendMovementToServer(transform.position, transform.rotation);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, syncedPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncedRotation, Time.deltaTime * 10f);
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
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
        if (Mathf.Abs(transform.up.y) < 0.5f && Mathf.Abs(transform.up.z) > 0.5f)
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

    private void CheckUpsideDown()
    {
        if (transform.up.y < -0.5f)
        {
            upsideDownTimer += Time.deltaTime;
            if (upsideDownTimer >= correctionDelay)
            {
                CorrectCarPosition();
            }
        }
        else
        {
            upsideDownTimer = 0f;
        }
    }

    private void CorrectCarPosition()
    {
        transform.position += Vector3.up;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        rolloverTimer = 0f;
        upsideDownTimer = 0f;
    }

    [ServerRpc]
    private void SendMovementToServer(Vector3 position, Quaternion rotation)
    {
        UpdateMovementOnClients(position, rotation);
    }

    [ObserversRpc]
    private void UpdateMovementOnClients(Vector3 position, Quaternion rotation)
    {
        syncedPosition = position;
        syncedRotation = rotation;
    }

    [ObserversRpc]
    public void CambiarColor(Color color)
    {
        Renderer carRenderer = GetComponent<Renderer>();
        if (carRenderer != null)
        {
            carRenderer.material.color = color;
        }
    }

    
    
    private IEnumerator FetchCarConfigFromAPI()
    {
        string apiUrl = "http://127.0.0.1:3000/config";


        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al obtener configuración del coche: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                CarConfig config = JsonUtility.FromJson<CarConfig>(json);

                motorForce = config.motorForce;
                steeringAngle = config.steeringAngle;
                brakeForce = config.brakeForce;

                Debug.Log("✅ Configuración del coche cargada desde la API.");
            }
        }
    }
}

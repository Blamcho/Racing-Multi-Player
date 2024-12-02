using FishNet.Object;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    public Transform target;  
    public Vector3 offset;    
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }

    public override void OnStartNetwork()
    {
        if (Owner.IsLocalClient)
        {
            
            Camera camera = Camera.main;
            if (camera != null)
            {
                CameraFollow cameraFollow = camera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTarget(transform);
                }
                else
                {
                    Debug.LogError("El componente CameraFollow no está adjunto a la cámara principal.");
                }
            }
            else
            {
                Debug.LogError("No se encontró una cámara con el tag 'MainCamera'.");
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
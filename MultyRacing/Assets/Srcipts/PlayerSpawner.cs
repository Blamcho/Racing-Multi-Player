using UnityEngine;
using FishNet.Object;

public class PlayerSpawner : NetworkBehaviour
{
    private SpawnManager spawnManager;

    private void Start()
    {
        if (IsServer)
        {
             //spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        }
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        if (IsServer)
        {
          
            spawnManager = FindObjectOfType<SpawnManager>();

            if (spawnManager != null)
            {
                // Obtener un punto de spawn disponible
                Transform spawnPoint = spawnManager.GetAvailableSpawnPoint();

                if (spawnPoint != null)
                {

                    transform.position = spawnPoint.position;
                    transform.rotation = spawnPoint.rotation;
                }
                else
                {
                    Debug.LogError("No se encontró un punto de spawn disponible.");
                }
            }
            else
            {
                Debug.LogError("No se encontró el SpawnManager en la escena.");
            }
        }
    }

    private void OnDestroy()
    {
        if (IsServer && spawnManager != null)
        {
            // Liberar el punto de spawn al desconectarse
            spawnManager.FreeSpawnPoint(transform);
        }
    }
}
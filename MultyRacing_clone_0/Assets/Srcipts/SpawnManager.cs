using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SpawnManager : NetworkBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>(); // Lista de puntos de spawn
    private List<bool> usedSpawnPoints; // Lista para rastrear qué puntos están ocupados

    private void Awake()
    {
        
        usedSpawnPoints = new List<bool>(new bool[spawnPoints.Count]);
    }

    /// <summary>
  
    /// </summary>
    /// <returns></returns>
    public Transform GetAvailableSpawnPoint()
    {
        for (int i = 0; i < usedSpawnPoints.Count; i++)
        {
            if (!usedSpawnPoints[i]) 
            {
                usedSpawnPoints[i] = true; // Marcar como usado
                return spawnPoints[i];
            }
        }

        Debug.LogWarning("No hay puntos de spawn disponibles.");
        return null; 
    }

    /// <summary>
  
    /// </summary>
    public void FreeSpawnPoint(Transform spawnPoint)
    {
        int index = spawnPoints.IndexOf(spawnPoint);
        if (index != -1)
        {
            usedSpawnPoints[index] = false;
        }
    }
}


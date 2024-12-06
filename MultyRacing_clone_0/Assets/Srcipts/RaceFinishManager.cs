using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class RaceFinishManager : NetworkBehaviour
{
    private bool raceFinished = false;

    public void PlayerCrossedFinishLine(NetworkConnection winner)
    {
        if (raceFinished) return;

        raceFinished = true;

     
        TargetShowResult(winner, "¡Ganaste la carrera!");

   
        foreach (var playerConn in ServerManager.Clients.Values)
        {
            if (playerConn != winner)
            {
                TargetShowResult(playerConn, "Perdiste la carrera.");
            }
        }
    }

    [TargetRpc]
    private void TargetShowResult(NetworkConnection conn, string message)
    {
        var resultText = GameObject.Find("RaceResultText").GetComponent<TMPro.TextMeshProUGUI>();
        resultText.text = message;
        resultText.gameObject.SetActive(true);

        
    }

   
}



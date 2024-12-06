using UnityEngine;
using FishNet.Object;

public class FinishLineTrigger : NetworkBehaviour
{
    [SerializeField] private RaceFinishManager raceFinishManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

      
        var networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsOwner)
        {
            raceFinishManager.PlayerCrossedFinishLine(networkObject.Owner);
        }
    }
}
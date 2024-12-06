using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using UnityEngine;
using FishNet.Object;

public class Meta : NetworkBehaviour
{
    
    bool ganador = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (IsServerStarted == false)
            return;
        
       if(!other.CompareTag("Player"))
           return;
       if (ganador)
           return;
       
     
   
      
       CarControllers  PGanador = other.gameObject.GetComponent<CarControllers>();
       
      other.GetComponent<CarControllers>().CambiarColor(Color.green);
     Dictionary<int, NetworkConnection> clientes =  base.ServerManager.Clients;
     foreach (var cliente in clientes)
     {
         if (cliente.Key != PGanador.OwnerId)
         {
             
         }
     }
      
      
      ganador = true;
      
    }
    
}

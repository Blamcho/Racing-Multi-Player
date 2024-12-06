using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using TMPro;
using   FishNet.Object.Synchronizing;
using UnityEditor;


public class contador : NetworkBehaviour
{
    public TextMeshProUGUI tiempoText;
   // readonly SyncTimer tiempoRestante = new SyncTimer();
    readonly SyncStopwatch tiempoTranscurrido = new SyncStopwatch();
    public TextMeshProUGUI tiempoTranscurridoText; 
    

    void Awake()
    {
      //  tiempoRestante.OnChange += OnTiempoRestanteChange;
        tiempoTranscurrido.OnChange += OnTiempoTrancurridoChange;
    }

    void OnTiempoTrancurridoChange(SyncStopwatchOperation op, float prev, bool asServe)
    {
        print($"tiempo trancurrio: {op} - {prev}");
        switch (op)
        {
            case SyncStopwatchOperation.Start:
                tiempoText.color = Color.white;
                break;
            case SyncStopwatchOperation.Pause:
                tiempoText.color = Color.yellow;
                break;
            case SyncStopwatchOperation.Unpause:
                break;
            case SyncStopwatchOperation.PauseUpdated:
                break;
            case SyncStopwatchOperation.Stop:
                tiempoText.color = Color.red;
                break;
            case SyncStopwatchOperation.StopUpdated:
                break;
        }
    }

   

    void Update()
    {
        tiempoTranscurrido.Update(Time.deltaTime);
        tiempoTranscurridoText.text = $"{tiempoTranscurrido.Elapsed:0:00}";

        if (Input.GetKeyDown(KeyCode.Y))
        {
           tiempoTranscurrido.StartStopwatch();
        }
        
    }

}







   


   
    


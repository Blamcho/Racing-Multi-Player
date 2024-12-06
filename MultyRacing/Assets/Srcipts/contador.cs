using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using TMPro;
using FishNet.Object.Synchronizing;

public class Contador : NetworkBehaviour
{
    public TextMeshProUGUI tiempoText; 
    readonly SyncStopwatch tiempoTranscurrido = new SyncStopwatch();
    public TextMeshProUGUI tiempoTranscurridoText; 
    public TextMeshProUGUI resultadoText; 

    private bool hasCrossedFinishLine = false;
    private bool isTimerRunning = true; 

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCounter();
    }

    void Awake()
    {
        tiempoTranscurrido.OnChange += OnTiempoTranscurridoChange;
    }

    [Server]
    private void StartCounter()
    {
        tiempoTranscurrido.StartStopwatch();
        isTimerRunning = true; 
    }

    void OnTiempoTranscurridoChange(SyncStopwatchOperation op, float prev, bool asServer)
    {
        switch (op)
        {
            case SyncStopwatchOperation.Start:
                tiempoText.color = Color.white;
                break;
            case SyncStopwatchOperation.Pause:
                tiempoText.color = Color.yellow;
                break;
        }
    }

    void Update()
    {
        if (!isTimerRunning) return;

        
        tiempoTranscurrido.Update(Time.deltaTime);
        tiempoTranscurridoText.text = $"{tiempoTranscurrido.Elapsed:0.00}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || hasCrossedFinishLine) return; 

        if (other.CompareTag("FinishLine")) 
        {
            hasCrossedFinishLine = true;
            
            if (tiempoTranscurrido.Elapsed <= 100f)
            {
                ShowResult("¡Ganaste!");
            }
            else
            {
                ShowResult("Perdiste.");
            }

            // Detiene el cronómetro para este jugador
            StopCounter();
        }
    }

    [Client]
    private void ShowResult(string message)
    {
        resultadoText.text = message; 
        resultadoText.gameObject.SetActive(true);
        
        Invoke(nameof(HideResult), 5f);
    }

    private void HideResult()
    {
        resultadoText.gameObject.SetActive(false);
    }

    private void StopCounter()
    {
        isTimerRunning = false;
    }
}

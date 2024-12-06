using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  TMPro;

public class PartidaItem : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI NombrePartida;
    [SerializeField] TextMeshProUGUI NumeroJugadores;
    EdgegapRelayManager edgegapRelayManager;

    public void SetUp(ApiResponse apiResponse, EdgegapRelayManager edgegapRelayManager)
    {
        NombrePartida.text = apiResponse.session_id;
        NumeroJugadores.text = apiResponse.session_users.Length.ToString();
            this.edgegapRelayManager = edgegapRelayManager;
    }

    public async void UnirPartida()
    {
      await  edgegapRelayManager.UnirPartidas( NombrePartida.text);
    }
}

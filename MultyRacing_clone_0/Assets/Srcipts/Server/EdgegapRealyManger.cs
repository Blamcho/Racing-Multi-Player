using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FishNet.Transporting;
using FishNet.Transporting.KCP.Edgegap;
using Newtonsoft.Json;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Networking;

public class EdgegapRelayManager : MonoBehaviour
{
   [SerializeField] private string relayToken;
   [SerializeField] private EdgegapKcpTransport KcpTransport;
   [SerializeField] Transform partdaitemcontainer;
   [SerializeField] GameObject Partida_ItemPrefab;
   [SerializeField] GameObject PartidasUIGameObject;
   
    bool isLocalHost;
    string sessionActualId;
      uint locaUserToKen;
   

   void ActulizarListaPartida(Sessions sessions)
   {
      foreach (Transform child in partdaitemcontainer)
      {
         Destroy(child.gameObject);
      }

      foreach (ApiResponse partidaData in sessions.sessions)
      {
         GameObject newitem = Instantiate(Partida_ItemPrefab, partdaitemcontainer);
         PartidaItem partidaItem = newitem.GetComponent<PartidaItem>();
         partidaItem.SetUp(partidaData, this);
      }
   }
   
  
   void Start()
   {
      // Suscribir eventos
      KcpTransport.OnServerConnectionState += OnServerConnectionStateChange;
      KcpTransport.OnClientConnectionState += OnClientConnectionStateChange;
      RefreshPartridas();
   }

   public async void CrearPartida()
   {
     // StartCoroutine(GetIP());
    await CrearPartidaAsync();
   }

   void OnServerConnectionStateChange(ServerConnectionStateArgs args)
   {
      switch (args.ConnectionState)
      {
         case LocalConnectionState.Stopped:
            print("Servidor detenido");
            break;
         case LocalConnectionState.Starting:
            print("Servidor iniciando...");
            break;
         case LocalConnectionState.Started:
            print("Servidor iniciado");
            break;
         case LocalConnectionState.Stopping:
            print("Servidor deteniéndose...");
            break;
      }
   }

   void OnClientConnectionStateChange(ClientConnectionStateArgs args)
   {
      switch (args.ConnectionState)
      {
         case LocalConnectionState.Stopped:
            SalirDeletarPartida();
            RefreshPartridas();
            PartidasUIGameObject.SetActive(true);
            break;
         case LocalConnectionState.Starting:
            PartidasUIGameObject.SetActive(false);
            
            break;
         case LocalConnectionState.Started:
      
            break;
         case LocalConnectionState.Stopping:
            print("Cliente desconectándose...");
            break;
         default:
            throw new AggregateException();
      }
   }

   void SalirDeletarPartida()
   {
      if (!string.IsNullOrWhiteSpace(sessionActualId))
      {
         if (isLocalHost)
         {
            BorrarPartida(sessionActualId);
         }
         else
         {
            AbandonarPartida();
         }
      }
      sessionActualId = null;
   }

   void OnApplicationQuit()
   {
      SalirDeletarPartida();
   }
   
   const string kEdgegapBaseURL = "https://api.edgegap.com/v1";
   HttpClient httpClient = new HttpClient();
   
   

   public async Task CrearPartidaAsync()
   {
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/ip");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
      UserIp userIp = JsonUtility.FromJson<UserIp>(response);
      print(userIp.public_ip);

      Users users = new Users
      {

         users = new List<User>()
      };   
      
      
      users.users.Add(new User(){ip = userIp.public_ip});
      
      string userJson = JsonUtility.ToJson(users);
      HttpContent content = new StringContent(userJson, Encoding.UTF8, "application/json");
      responseMessage = await httpClient.PostAsync($"{kEdgegapBaseURL}/relays/sessions", content);
      response = await responseMessage.Content.ReadAsStringAsync();
      ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);
      print("Sesson:" + apiResponse.session_id);
      

      while (!apiResponse.ready)
      {
         await Task.Delay(2500);
          responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/relays/sessions/{apiResponse.session_id}");
          response = await  responseMessage.Content.ReadAsStringAsync();
          apiResponse = JsonUtility.FromJson<ApiResponse>(response);
          print(response);
      }
      
      ConetarnosAPartida(apiResponse);
   }
   
   
   void ConetarnosAPartida(ApiResponse apiResponse)
   {

      uint userToken = 0;
      if (apiResponse.session_users != null)
      {
         userToken = apiResponse.session_users[0].authorization_token;
         isLocalHost = true;
      }

      else
      {
         userToken = apiResponse.session_user.authorization_token;
         isLocalHost = false;
      }
      locaUserToKen = userToken;
      
      EdgegapRelayData relayData = new EdgegapRelayData(
         apiResponse.relay.ip,
         apiResponse.relay.ports.server.port, 
         apiResponse.relay.ports.client.port, 
         userToken,
         apiResponse.authorization_token
      );
      
      sessionActualId = apiResponse.session_id;
      
      
      KcpTransport.SetEdgegapRelayData(relayData);
      if(isLocalHost)
      KcpTransport.StartConnection(true);
      KcpTransport.StartConnection(false);
   }

   public async void RefreshPartridas()
   {
      await GetTodasLas_PartidasAsync();
   }
   
   async Task GetTodasLas(string session_id)
   {
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/relays/sessions");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
   }
   
   async Task GetTodasLas_PartidasAsync()
   {
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/relays/sessions");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
      print(response);
      
      Sessions sessions = JsonUtility.FromJson<Sessions>(response);
      ActulizarListaPartida(sessions);
   }

   public async Task UnirPartidas(string session_id)
   {
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/ip");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
      UserIp userIp = JsonUtility.FromJson<UserIp>(response);

      JoinSession joinSessionData = new JoinSession()
      {
         session_id = session_id,
         user_ip = userIp.public_ip
      };
      
      
      string userJson = JsonUtility.ToJson(joinSessionData);
      HttpContent content = new StringContent(userJson, Encoding.UTF8, "application/json");
      responseMessage = await httpClient.PostAsync($"{kEdgegapBaseURL}/relays/sessions:authorize-user", content);
      response = await responseMessage.Content.ReadAsStringAsync();
      ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);
      
      ConetarnosAPartida(apiResponse);
   }

   async Task AbandonarPartida()
   {
      LeaveSession leaveSessionData = new LeaveSession()
      {
         session_id = sessionActualId,
         authorization_token = locaUserToKen
      };
      
      string leaveSessionJson = JsonUtility.ToJson(leaveSessionData);
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpContent content = new StringContent(leaveSessionJson, Encoding.UTF8, "application/json");
       httpClient.PostAsync($"{kEdgegapBaseURL}/relays/sessions:revoke-user", content);
      
   }

   async Task BorrarPartida(string session_id)
   {
      HttpResponseMessage responseMessage = await httpClient.DeleteAsync($"{kEdgegapBaseURL}/relays/sessions{session_id}");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
   }

   [ContextMenu("Borrar Partida")]
   async void DevBorrartodasPartidas()
   {
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/relays/sessions");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
      print(response);
      
      Sessions sessions = JsonUtility.FromJson<Sessions>(response);
      foreach (ApiResponse partida in sessions.sessions)
      {
         await  BorrarPartida(partida.session_id);
         print("todas las partidas fueron borradas");
      }
   }
   
   
   [ContextMenu("Obtener informacion")]
   void ObtenerInformacionPartida()
   {
      Task.Run(EsperaryHoster);
   }

   async Task EsperaryHoster()
   {
      string session_id = "ac28e7f11828-S";
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "token",relayToken);
      HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseURL}/relays/sessions/{session_id}");
      string  response = await  responseMessage.Content.ReadAsStringAsync();
      print(response);
   }

 

   IEnumerator GetIP()
   {
      using UnityWebRequest unityWebRequest = new UnityWebRequest($"{kEdgegapBaseURL}/ip", "GET");
      unityWebRequest.SetRequestHeader("Authorization", relayToken);
      unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
      
      yield return unityWebRequest.SendWebRequest();

      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
         Debug.LogError("Error al obtener el IP");
         Debug.LogError(unityWebRequest.error);
         yield break;
      }
      
      string  response = unityWebRequest.downloadHandler.text;
      print(response);
      UserIp userIp = JsonUtility.FromJson<UserIp>(response);
      print(userIp);
      
   }

   IEnumerator CrearParrtidaCoroutine()
   {
      using UnityWebRequest unityWebRequest = new UnityWebRequest($"{kEdgegapBaseURL}/v1/relays/sessions", "POST");
      unityWebRequest.SetRequestHeader("Authorization", relayToken);

      unityWebRequest.SendWebRequest();

      if (unityWebRequest.result != UnityWebRequest.Result.Success)
      {
         Debug.LogError("Error al crear partida");
         Debug.LogError(unityWebRequest.error);
         yield break;
      }
      
      string  response = unityWebRequest.downloadHandler.text;
      print(response);
      
      ApiResponse aoi = JsonUtility.FromJson<ApiResponse>(response);
      
      

   }
   
}

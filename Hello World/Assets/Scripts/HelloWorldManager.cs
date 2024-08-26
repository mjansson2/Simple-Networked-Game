using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : NetworkBehaviour
    {

        [SerializeField] GameObject playerPrefab1;
        [SerializeField] GameObject playerPrefab2;
        [SerializeField] GameObject rainPrefab;

        private int clientCount = 0;
        private ulong player1ClientId;

        [SerializeField] GameObject cakePrefab1;
        [SerializeField] GameObject cakePrefab2;

        private int cake1Count = 0;
        private int cake2Count = 0; 

        Vector2 cakeSpawnArea = new Vector2(10f, 10f);

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;

            }
        }

        private void OnServerStarted()
        {
            SpawnRain();
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                clientCount++;

                GameObject prefabToSpawn = clientCount == 1 ? playerPrefab1 : playerPrefab2;

                if (clientCount == 1)
                {
                    player1ClientId = clientId;
                }
                if (prefabToSpawn != null)
                {
                    GameObject playerObject = Instantiate(prefabToSpawn);
                    playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                }

            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (clientId == player1ClientId)
            {
                clientCount = 0;
            }
            else
            {
                clientCount = 1;
            }
        }
            
            
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                if (GUILayout.Button("Get Cake"))
                {
                    SubmitCakeSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
                    GetCakeCount(cakePrefab1);
                    GetCakeCount(cakePrefab2);

                    Debug.Log($"Kitty has: {cake1Count} cakes & Squirelly has: {cake2Count} cakes"); 
                }

                if (GUILayout.Button("Quit Game"))
                {
                    Application.Quit();
                }

            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {

            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        private void SpawnRain()
        {
            GameObject prefabToSpawn = rainPrefab;
            if (prefabToSpawn != null)
            {
                GameObject rainObject = Instantiate(prefabToSpawn);
                rainObject.GetComponent<NetworkObject>().Spawn();
            }

        }

        [Rpc(SendTo.Server)]
        public void SubmitCakeSpawnServerRpc(ulong clientId, RpcParams rpcParams = default)
        {
            SpawnCake(NetworkManager.Singleton.LocalClientId == clientId ? cakePrefab1 : cakePrefab2);
        }


        public void SpawnCake(GameObject cakePrefab)
        {
            GameObject cakeObject;

            Vector3 spawnPosition = new Vector3(Random.Range(-cakeSpawnArea.x, cakeSpawnArea.x), 3f,
                                                Random.Range(-cakeSpawnArea.y, cakeSpawnArea.y));

            cakeObject = Instantiate(cakePrefab, spawnPosition, Quaternion.identity);
            cakeObject.GetComponent<NetworkObject>().Spawn();

            if (cakePrefab == cakePrefab1)
            {
                cake1Count++;
            }
            else if (cakePrefab == cakePrefab2)
            {
                cake2Count++;
            }

        }

        public int GetCakeCount(GameObject cakePrefab)
        {
            if (cakePrefab == cakePrefab1)
            {
                return cake1Count;

            }
            else if (cakePrefab == cakePrefab2)
            {
                return cake2Count;
            }
            return 0;
        }

    }

}


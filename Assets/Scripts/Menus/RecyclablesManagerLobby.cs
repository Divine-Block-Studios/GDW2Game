using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class RecyclablesManagerLobby : NetworkManager
    {
        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }



        [Scene] [SerializeField] private string menuScene;
        [Scene] [SerializeField] private string lobbyScene;
        
        [Header("Room")] [SerializeField] private RecyclablesPlayerLobby roomPlayPrefab;

        public override void OnStartServer()
        {
            print("ServerStarted");
            spawnPrefabs = Resources.LoadAll<GameObject>("Prefabs/SpawnablePrefabs").ToList();
        }

        public override void OnStartClient()
        {
            print("ClientStarted");
            List<GameObject> spawnablePrefabs = Resources.LoadAll<GameObject>("Prefabs/SpawnablePrefabs").ToList();

            foreach (GameObject prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            print("Client Connected");
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.LoadScene(lobbyScene);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            print("Client Disconnected");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            print("Connecting To Server");
            if (numPlayers >= maxConnections)
            {
                print("Bad");
                //Instead of doing this, check if they're equal, then remove from list
                conn.Disconnect();
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            print("Adding Player: " + conn);
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                RecyclablesPlayerLobby roomInstance = Instantiate(roomPlayPrefab);
                NetworkServer.AddPlayerForConnection(conn, roomInstance.gameObject);
            }
        }
    }
}

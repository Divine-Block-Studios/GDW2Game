using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class RecyclablesManagerLobby : NetworkManager
    {
        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            _roomInstance = new List<RecyclablesPlayerLobby>();
        }

        [Scene] [SerializeField] private string menuScene;
        [Scene] [SerializeField] private string lobbyScene;
        [SerializeField] private Vector3 [] playerSpawns;
        private List<RecyclablesPlayerLobby> _roomInstance;

        [Header("Room")] [SerializeField] private RecyclablesPlayerLobby roomPlayPrefab;
        [SerializeField] private Sprite charSprite;
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
            for (int i = 0; i < _roomInstance.Count; i++)
            {
                _roomInstance[i].transform.position = playerSpawns[i];
            }
            print("Client Connected");
            SceneManager.LoadScene(lobbyScene);
            
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            print("Client Disconnected");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            print("Connecting To Server: " + numPlayers);
            if (numPlayers >= maxConnections)
            {
                print("Bad");
                //Instead of doing this, check if they're equal, then remove from list
                conn.Disconnect();
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            _roomInstance.Add(Instantiate(roomPlayPrefab));
            _roomInstance[_roomInstance.Count-1].gameObject.name = "OBJ " + numPlayers;
            for (int i = 0; i < _roomInstance.Count; i++)
            {
                _roomInstance[i].transform.position = playerSpawns[i];
            }

           
            DontDestroyOnLoad(_roomInstance[_roomInstance.Count-1]);
            NetworkServer.AddPlayerForConnection(conn, _roomInstance[_roomInstance.Count-1].gameObject);
            print("Adding Player: TRUE" + conn + " - " + numPlayers);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            print("Disconnecting player: " + conn +  " - " +  numPlayers);
        }
    }
}

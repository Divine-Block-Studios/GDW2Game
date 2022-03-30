using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menus
{
    public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
    {
        // Start is called before the first frame update
        [SerializeField] private TMP_InputField lobbyCode;
        [SerializeField] private TextMeshProUGUI textObj;

        private bool isHost;
        private string code;
        public void Awake()
        {
            //PhotonNetwork.OfflineMode = false;
        }

        public void JoinOrCreateRoom(bool createRoom)
        {
            Debug.Log("0 Connecting: " + createRoom);
            //PhotonNetwork.OfflineMode = false;
            isHost = createRoom;
            if (!isHost)
            {
                code = lobbyCode.text;
            }
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Successfully connected to master");
            PhotonNetwork.NickName = Settings.settings.Name;
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            print("Reached");
            if (isHost)
            {
                GenerateCode();
                PhotonNetwork.CreateRoom(code, new RoomOptions { MaxPlayers = 6} );
            }
            else
            {
                PhotonNetwork.JoinRoom(code);
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            //Generate new code.
            Debug.Log("Code invalid.");
            GenerateCode();
            PhotonNetwork.CreateRoom(code, new RoomOptions { MaxPlayers = 6} );
        }

        private void GenerateCode()
        {
            code = "";
            for (int i = 0; i < 6; i++)
            {
                code += Random.Range(0, 10);
            }
            print("Generated code: " + code);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //Invalid room code.
            textObj.text = message + " :: " + returnCode;
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            //if Name is the same, add a (1) to name. If name is too long, Replace last three chars for (1)
            Debug.Log("Hello");
            print(newPlayer.ActorNumber + "::" + newPlayer.NickName + " - has joined lobby.");
        }
        public override void OnJoinedRoom()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            if (isHost)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LoadLevel(1);
            }
            print("ROOM: " + PhotonNetwork.CurrentRoom.Name);
            textObj.text = PhotonNetwork.CurrentRoom.Name + " - " + PhotonNetwork.CurrentRoom.Players.Count;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            textObj.text = "Disconnected.";
        }
    }
}

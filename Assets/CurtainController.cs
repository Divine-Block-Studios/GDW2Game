using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurtainController : MonoBehaviourPunCallbacks
{
    private int playerCount;
    private int readyCount;
    private bool imReady;
    public Action onComplete = null;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            //if (!PhotonNetwork.IsConnected || scene.name == "InLobbyScene")
               // return;
            print("Scene loaded Check is working");
            imReady = true;
            //photonView.RPC("CheckPlayersLoaded", RpcTarget.AllBuffered);
        };
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //if(PhotonNetwork.IsMasterClient)
            //photonView.RPC("UpdatePlayerCount", RpcTarget.Others, ++playerCount);
        print("Player entered room CC");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       // if(PhotonNetwork.IsMasterClient)
            //photonView.RPC("UpdatePlayerCount", RpcTarget.Others, --playerCount);
        //if(imReady)
            //photonView.RPC("UpdatePlayerCount", RpcTarget.Others, --readyCount);
        
        print("Player left room CC");
    }


    [PunRPC]
    private void UpdatePlayerCount(int val)
    {
        playerCount = val;
    }

    [PunRPC]
    private void UpdateReadyCount(int val)
    {
        readyCount = val;
    }

    [PunRPC]
    private void CheckPlayersLoaded()
    {
        if (playerCount == readyCount)
        {
            readyCount = 0;
            imReady = false;
        }
        //Open Curtains
        StaticHelpers.Curtains(() => Time.timeScale = 1);
    }
}

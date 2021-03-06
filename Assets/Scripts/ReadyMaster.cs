using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class ReadyMaster : MonoBehaviourPunCallbacks
{
    private int playerCount;
    private int readyCount;
    private bool imReady;
    public UnityEvent onComplete = null;

    void Awake()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        print("Ready Master is Awake");
        photonView.RPC("UpdateReadyCount", RpcTarget.AllBuffered, +1);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered);
        print("Player entered room CC");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered);
        if(imReady)
            photonView.RPC("UpdateReadyCount", RpcTarget.AllBuffered, -1);
        
        print("Player left room CC");
    }


    [PunRPC]
    private void UpdatePlayerCount()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        CheckPlayersLoaded();
    }

    [PunRPC]
    private void UpdateReadyCount(int val)
    {
        readyCount += val;
        CheckPlayersLoaded();
        print("Testing: Player Readied");
    }

    private void CheckPlayersLoaded()
    {
        print("Checking Ready Count: " + playerCount + " | " + readyCount);
        if (playerCount == readyCount && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("DetectReady", RpcTarget.All);
        }
    }

    [PunRPC]
    private void DetectReady()
    {
        readyCount = 0;
        imReady = false;
        print("All players are ready");
            
        //Open Curtains
        StaticHelpers.Curtains(() => onComplete.Invoke());
    }
}

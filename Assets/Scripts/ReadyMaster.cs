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
        print("Ready Master is Awake");
        photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, ++readyCount);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, ++playerCount);
        print("Player entered room CC");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, --playerCount);
        if(imReady)
            photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, --readyCount);
        
        print("Player left room CC");
    }


    [PunRPC]
    private void UpdatePlayerCount(int val)
    {
        playerCount = val;
        CheckPlayersLoaded();
    }

    [PunRPC]
    private void UpdateReadyCount(int val)
    {
        readyCount = val;
        CheckPlayersLoaded();
        print("Testing: Player Readied");
    }

    private void CheckPlayersLoaded()
    {
        
        if (playerCount == readyCount)
        {
            readyCount = 0;
            imReady = false;
            print("All players are ready");
            
            //Open Curtains
            StaticHelpers.Curtains(() => onComplete.Invoke());
        }
    }
}

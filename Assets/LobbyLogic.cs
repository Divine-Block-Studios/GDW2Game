using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLogic : MonoBehaviour
{
    public TextMeshProUGUI roomCode;
    [SerializeField] private Vector3[] playerPoints;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private Transform canvas;

    private void Awake()
    {
        roomCode.text = PhotonNetwork.CurrentRoom.Name;

        GameObject go = PhotonNetwork.Instantiate("Prefabs/"+lobbyPrefab.name, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(canvas);
        print(PhotonNetwork.CurrentRoom.PlayerCount - 1);
        go.transform.localPosition = playerPoints[PhotonNetwork.CurrentRoom.PlayerCount - 1];

    }
    
    
    
    
}

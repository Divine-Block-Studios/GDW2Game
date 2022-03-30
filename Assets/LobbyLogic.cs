using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLogic : MonoBehaviourPunCallbacks
{
    [Header("Player Information")]
    public TextMeshProUGUI roomCode;
    [SerializeField] private Vector3[] playerPoints;
    [SerializeField] private GameObject lobbyPrefab;
    
    [Header("Host Information")]
    [SerializeField] private GameObject serverSettingsPrefab;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject disableConnectionsButton;
    [SerializeField] private GameObject showCodeButton;
    [SerializeField] private GameObject code;
    
    [Header("Host Tools")]
    [SerializeField] private Sprite roomLocked;
    [SerializeField] private Sprite roomUnlocked;
    [SerializeField] private Sprite hideRoomCode;
    [SerializeField] private Sprite showRoomCode;

    private Image lockImage;
    private Image visibilityImage;
    private Button lockButton;
    private Button visibilityButton;
    private bool roomIsOpen;
    private bool roomCodeIsVisible;
    
    private void Awake()
    {
        roomCode.text = PhotonNetwork.CurrentRoom.Name;
        GameObject go = PhotonNetwork.Instantiate("Prefabs/" + lobbyPrefab.name, Vector3.zero, Quaternion.identity);
        go.transform.GetChild(0).localPosition = playerPoints[PhotonNetwork.CurrentRoom.PlayerCount - 1];
        print(PhotonNetwork.CurrentRoom.PlayerCount - 1);

        if (PhotonNetwork.IsMasterClient)
        {
            serverSettingsPrefab.SetActive(true);
            startGameButton.SetActive(true);
            disableConnectionsButton.SetActive(true);
            showCodeButton.SetActive(true);
            
            lockImage = disableConnectionsButton.GetComponent<Image>();
            visibilityImage = showCodeButton.GetComponent<Image>();
            lockButton = disableConnectionsButton.GetComponent<Button>();
            visibilityButton = showCodeButton.GetComponent<Button>();

            roomCodeIsVisible = !Settings.settings.HideCodeOnStart;
            visibilityImage.sprite = roomCodeIsVisible ? showRoomCode:hideRoomCode;

            lockButton.onClick.AddListener(ToggleRoomAvailability);
            visibilityButton.onClick.AddListener(ToggleRoomCodeVisibility);
        }
    }

    private void ToggleRoomAvailability()
    {
        
        roomIsOpen = !roomIsOpen;
        Debug.Log("Toggling room availability: " + roomIsOpen);
        PhotonNetwork.CurrentRoom.IsOpen = roomIsOpen;
        lockImage.sprite = roomIsOpen?roomUnlocked:roomLocked;
    }
    private void ToggleRoomCodeVisibility()
    {
        roomCodeIsVisible = !roomCodeIsVisible;
        Debug.Log("Toggling roomcode visibility: " + roomCodeIsVisible);
        visibilityImage.sprite = roomCodeIsVisible ? showRoomCode:hideRoomCode;
        photonView.RPC("ShowRoomCode", RpcTarget.All, roomCodeIsVisible);
    }

    [PunRPC]
    private void ShowRoomCode(bool visibility)
    {
        print("Show Room Code RPC");
        code.SetActive(visibility);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //Close lobby.
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.Disconnect();
    }
}

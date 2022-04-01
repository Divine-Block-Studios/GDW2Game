using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyLogic : MonoBehaviourPunCallbacks
{
    [Header("Player Information")]
    public TextMeshProUGUI roomCode;
    [SerializeField] private Vector3[] playerPoints;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private GameObject timerObject;
    
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
    [SerializeField] private TextMeshProUGUI boardNameUI;
    [SerializeField] private Image mapImage;
    [SerializeField] private int countDownSeconds = 5;


    private Image lockImage;
    private Image visibilityImage;
    private Button lockButton;
    private Button visibilityButton;
    private bool roomIsOpen;
    private bool roomCodeIsVisible;
    private bool canCountDown;

    public string BoardName { get; set; }
    public Sprite MapSprite { get; set; }

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
            BoardName = boardNameUI.text;
            MapSprite = mapImage.sprite;
            

            roomCodeIsVisible = !Settings.settings.HideCodeOnStart;
            visibilityImage.sprite = roomCodeIsVisible ? showRoomCode : hideRoomCode;

            lockButton.onClick.AddListener(ToggleRoomAvailability);
            visibilityButton.onClick.AddListener(ToggleRoomCodeVisibility);
            startGameButton.GetComponent<Button>().onClick.AddListener(StartGame);
        }
        else
        {  
            //Fix issue w/ showing the hide button. Beep beep boop boop ba. Custom aids.
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
        SceneManager.LoadScene(0);
        PhotonNetwork.Disconnect();
    }
    
    private void StartGame()
    {
        photonView.RPC("StartGameRPC", RpcTarget.All);
    }

    [PunRPC]
    private void CancelGameRPC()
    {
        canCountDown = false;
        GameCancelled(3);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        timerObject.SetActive(true);
        canCountDown = true;
        CountDown(countDownSeconds);
    }

    private async void CountDown(int seconds)
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        float curTime = seconds + 1;

        while (curTime > 0 && canCountDown)
        {
            curTime -= Time.deltaTime;
            timerObject.GetComponent<TextMeshProUGUI>().text = curTime < 1 ? "STARTING" : ((int)curTime).ToString();
            await Task.Yield();
        }
        timerObject.SetActive(false);
        if (canCountDown && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(boardNameUI.text);
        }

    }

    private async void GameCancelled(int durationSeconds)
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        float curTime = durationSeconds + 1;

        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            if (durationSeconds - curTime < 1)
            {
                timerObject.GetComponent<TextMeshProUGUI>().text = "CANCELLED";
            }
            await Task.Yield();
        }
        timerObject.SetActive(false);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Board;
using Board.Tiles;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    public bool showTiles;
    [SerializeField] private GameObject uiSpinner;
    [SerializeField] private GameObject uiSelector;
    [SerializeField] private ToggleScoreBoard tsb;
    [SerializeField] private Transform DEBUG_SpinnerParent;
    [SerializeField] private Transform DEBUG_SelectorParent;

    [SerializeField] private Transform[] playerSpawnPoints;

    //May make sense to move this into BoardController class
    [Header("Player Objects")] 
    [SerializeField] private Transform playerParentObj;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private TextMeshPro diceObj;
    [SerializeField] private Transform shredderObj;
    

    [Header("Game Elements")]
    [SerializeField] private GameObject dice;

    [SerializeField] private GameObject rollDiceButton;
    [SerializeField] private Transform throwFromA;
    [SerializeField] private Transform throwFromB;
    [SerializeField] private Tile startTile;
    
    [Header("Game GameSettings")]
    [SerializeField] private ushort startingCoins;

    [SerializeField] private float timeBeforeForcedRoll;

    [SerializeField] private int forceNum = 0;
    
    [SerializeField] private GameObject enemy;

    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => players[curTurn];
    
    public BoardPlayer MyPlayer
    {
        get
        {
            for(int i = 0;  i < players.Length; i++)
                if(players[i].name == PhotonNetwork.LocalPlayer.NickName)
                    return players[i];
            return null;
        }
    }

    public Transform CameraArm;
    public static GameManager gameManager { get; set; }

    private byte curRound;
    private byte nextShreddingRound;
    private int curTurn;
    private int diceRemainder;
    
    public BoardPlayer [] players;
    public int DiceRemainder => diceRemainder;

    //Move into board player;
    private bool _inMenu;
    public bool InMenu => _inMenu;

    public bool isEnabled = false;

    public AwardableEvents[] DEBUGevts;

    public Item [] playersAsItems;

    private void Awake()
    {
        
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }

        //CreateSelectionUI(DEBUGevts, true, false, null);
        //return;
        players = new BoardPlayer[PhotonNetwork.CurrentRoom.PlayerCount];
        playersAsItems = new Item[players.Length];
        if (PhotonNetwork.IsMasterClient)
        {
            print("Seeing master");
            
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //Hash table needs to save player selected prefab.name (Load custom player models)
                players[i] = PhotonNetwork
                    .Instantiate("Prefabs/Map Assets/Players/Character" + i, Vector3.zero, quaternion.identity)
                    .GetComponentInChildren<BoardPlayer>();
                players[i].transform.position = playerSpawnPoints[i].position + new Vector3(0, players[i].GetComponent<RectTransform>().rect.height + 0.3f, 0);
                players[i].transform.LookAt(enemy.transform.position);
                
            }
            photonView.RPC("UpdatePlayerArray", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void UpdatePlayerArray()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Player");
        print(arr.Length + " - " + players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            print(i);
            players[i] = arr[i].GetComponent<BoardPlayer>();
            players[i].name = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;
            players[i].transform.GetChild(0).GetComponent<TextMeshPro>().text = players[i].name;
            players[i].currentTile = startTile;
            players[i].coins = startingCoins;
            
            playersAsItems[i] = ScriptableObject.CreateInstance<Item>();

            playersAsItems[i].name = PhotonNetwork.CurrentRoom.Players[i + 1].NickName;
            playersAsItems[i].icon = players[i].playerImg;
            playersAsItems[i].awardName = playersAsItems[i].name;
            
            print("test");
            if (PhotonNetwork.LocalPlayer.NickName == players[i].name)
            {
                print("I found my guy!");
                players[i].GetComponent<BoardInputControls>().Init();
                players[i].photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }

    public void BeginGame()
    {
        isEnabled = true;

        diceObj.transform.SetParent(GetCurrentPlayer.transform);
        shredderObj.SetParent(GetCurrentPlayer.transform);
        
        UpdateUIElements();
        

        if (MyPlayer == GetCurrentPlayer)
        {
            //Show dice roll element.
            rollDiceButton.SetActive(true);
            
            //If local player == allow item use. (You don't start w/ an item)
            //playerObject.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => GetCurrentPlayer.UseItem());
        }

        
    }

    public void RollDice()
    {
        //SAFETY
        if (MyPlayer != GetCurrentPlayer)
            return;
        
        rollDiceButton.SetActive(false);
        
        if (forceNum > 0)
        {
            CheckDice(forceNum);
            return;
        }

        Vector3 A = throwFromA.position;
        Vector3 B = throwFromB.position;
        
        Vector3 startLoc = new Vector3(Random.Range(A.x, B.x), Random.Range(A.y, B.y), Random.Range(A.z, B.z));
        print(A + " - " + B + " - " +Random.Range(A.x, B.x) + " = " + Random.Range(A.y, B.y) + " = " + Random.Range(A.z, B.z));
        
        PhotonNetwork.Instantiate("Prefabs/"+dice.name, startLoc, Quaternion.identity).GetComponent<DiceScript>().OnCompleted = CheckDice;
    }

    private void CheckDice(int num)
    {
        Debug.Log("Checking the dice from GM: " + num);
        diceRemainder = num;
        diceObj.gameObject.SetActive(true);
        diceObj.text = num.ToString();
        GetCurrentPlayer.MoveToTile(GetCurrentPlayer.currentTile.NextTile);
    }

    public void UpdateCamera()
    {
        //TODO: Make this not called every frame, only while moving and when swapping views.
        //While in game call this... While player is moving call this.
        CameraArm.transform.position = GetCurrentPlayer.transform.position;
    }

    private void Update()
    {
        if(isEnabled)
            UpdateCamera();
    }

    void EliminatePlayer()
    {
        
    }

    void EndRound()
    {
        if (++curRound == nextShreddingRound)
        {
            EliminatePlayer();
        }
    }

    void EndTurn()
    {
        curTurn = ++curTurn % players.Length;
        if (curTurn % players.Length == 0)
        {
            //Then the full round is complete
            EndRound();
        }

        UpdateUIElements();
        //
    }

    //This is called after a tile is landed on
    public void EndAction(Tile nextTile, bool costAction)
    {
        if (costAction)
        {
            Debug.Log("End Action: " + (diceRemainder - 1));
            if (--diceRemainder == 0)
            {
                diceObj.gameObject.SetActive(false);
                EndTurn();
                return;
            }
            diceObj.text = diceRemainder.ToString();
        }
        //?
        GetCurrentPlayer.MoveToTile(nextTile);
    }

    //Somewhat complicated generic function. Essentially takes a list of objects and formats them in an appropriate manner./
    //Then the selected object is returned (val) with a state. 
    //If the UI is random, the user isn't actually allowed to select the option, but it should be dynamic for the sake of it's more interesting.
    public void CreateSelectionUI(AwardableEvents[] objects, bool isSpinner, bool shouldShuffle, BoardPlayer ply, int randomItemsToDisplay = 1, Action onComplete = null)
    {
        GameObject go;
        if(shouldShuffle)
            StaticHelpers.Shuffle(objects);
        if (isSpinner)
        {
            //Create UI Spinner
            if (PhotonNetwork.IsMasterClient)
            {
                go = PhotonNetwork.Instantiate("Prefabs/Map Assets/" + uiSpinner.name, DEBUG_SpinnerParent.position, Quaternion.identity);
                SpinnerScript s = go.GetComponentInChildren<SpinnerScript>();
                if (objects[0] == playersAsItems[0])
                {
                    s.Init(ply, onComplete);
                }
                else
                {
                    s.Init(objects, ply, onComplete);
                }
            }
            //return s.Init(objects, spawnedUILoc);
        }
        else
        {
            //Create UI SelectorScript
            go = Instantiate(uiSelector, DEBUG_SelectorParent);
            SelectorScript s = go.GetComponent<SelectorScript>();
            _inMenu = true;
            //Safe cast into items.
            s.Init(objects, ply, randomItemsToDisplay, onComplete);
        }
    }
    
    public void LoadMiniGame(string gameName)
    {
        //Complete all tasks BEFORE loading.
        SceneManager.LoadScene(gameName);
    }

    public void UpdateUIElements()
    { 
        Transform plyIcon = playerObject.transform.GetChild(0);
        plyIcon.GetComponent<Image>().sprite = GetCurrentPlayer.playerImg;
        plyIcon.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCurrentPlayer.gameObject.name;
        playerObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCurrentPlayer.coins.ToString();
        Item i = GetCurrentPlayer.Item;
        if (i != null)
        {
            //If the player has an item
            Transform item =playerObject.transform.GetChild(0).GetChild(2);
            item.gameObject.SetActive(true);
            item.GetChild(0).GetComponent<Image>().sprite = i.icon;
            item.GetChild(1).GetComponent<TextMeshProUGUI>().text = i.awardName;
        }
        else
        {
            playerObject.transform.GetChild(2).gameObject.SetActive(false);
        }
        tsb.UpdateScoreBoard();
    }

    public BoardPlayer GetRandomPlayer(BoardPlayer ignore = null)
    {
        BoardPlayer rng = players[Random.Range(0, players.Length)];
        
        //Kind a cringe way to do this because in theory it can go forever
        if (ignore == rng)
        {
            if (players.Length == 0)
            {
                Debug.LogError("FATAL: infinite loop, ignored players is only player");
            }
            return GetRandomPlayer(ignore);
        }
        return rng;
    }
}

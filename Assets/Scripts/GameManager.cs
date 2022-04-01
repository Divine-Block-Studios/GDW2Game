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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Tile debugStartTile;
    public bool showTiles;
    [SerializeField] private GameObject uiSpinner;
    [SerializeField] private GameObject uiSelector;
    [SerializeField] private Transform DEBUG_SpinnerParent;
    [SerializeField] private Transform DEBUG_SelectorParent;

    //May make sense to move this into BoardController class
    [Header("Player Objects")] 
    [SerializeField] private Transform playerParentObj;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private TextMeshPro diceObj;
    [SerializeField] private Transform shredderObj;

    [Header("Game Elements")]
    [SerializeField] private GameObject dice;
    [SerializeField] private Transform throwFromA;
    [SerializeField] private Transform throwFromB;

    [Header("Game GameSettings")]
    [SerializeField] private ushort startingCoins;

    [SerializeField] private float timeBeforeForcedRoll;

    [SerializeField] private int forceNum = 0;

    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => players[curTurn];

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

    private bool isEnabled = false;

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

        if (PhotonNetwork.IsMasterClient)
        {
            print("Seeing master");
            players = new BoardPlayer[PhotonNetwork.CurrentRoom.PlayerCount];
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //Hash table needs to save player selected prefab.name
                players[i] = PhotonNetwork.Instantiate("Prefabs/Map Assets/Players/"+playerObject.name, Vector3.zero, quaternion.identity).GetComponentInChildren<BoardPlayer>();
            }
        }
    }
    public void BeginGame()
    {
        diceObj.transform.SetParent( players[0].transform);
        shredderObj.SetParent( players[0].transform);
        
        UpdateUIElements();
        
        GetCurrentPlayer.currentTile = debugStartTile;

        foreach (BoardPlayer player in players)
        {
            player.transform.position = debugStartTile.transform.position - player.GetHeight;
        }
        //DEBUG
        RollDice();
        //If local player == Current player, use item.
        playerObject.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => GetCurrentPlayer.UseItem());
    }

    private void RollDice()
    {
        if (forceNum > 0)
        {
            CheckDice(forceNum);
            return;
        }

        Vector3 A = throwFromA.position;
        Vector3 B = throwFromB.position;
        
        Vector3 startLoc = new Vector3(Random.Range(A.x, B.x), Random.Range(A.y, B.y), Random.Range(A.z, B.z));
        print(A + " - " + B + " - " +Random.Range(A.x, B.x) + " = " + Random.Range(A.y, B.y) + " = " + Random.Range(A.z, B.z));
        
        Instantiate(dice, startLoc, Quaternion.identity).GetComponent<DiceScript>().OnCompleted = CheckDice;
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
            go = Instantiate(uiSpinner, DEBUG_SpinnerParent);
            SpinnerScript s = go.GetComponent<SpinnerScript>();
            s.Init(objects, ply, onComplete);
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
        Transform plyIcon = playerObject.transform.GetChild(0).GetChild(0);
        plyIcon.GetComponent<Image>().sprite = GetCurrentPlayer.playerImg;
        plyIcon.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCurrentPlayer.gameObject.name;
        playerObject.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCurrentPlayer.coins.ToString();
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
            playerObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
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

    public void Activate()
    {
        AwardableEvents[] playersAsAwards = new AwardableEvents[players.Length];
        for (int i = 0; i < playersAsAwards.Length; i++)
        {
            print("Trying to set player image of player: " + i); // Error on player 0. Img not set. [ply not even appearing.]
            playersAsAwards[i].icon = players[i].playerImg;
        }
        CreateSelectionUI(playersAsAwards, true, false, null, 1, () =>
        {
            print("Done Spinning.");
            isEnabled = true;
        });
    }
}

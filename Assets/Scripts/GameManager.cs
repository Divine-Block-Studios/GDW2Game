using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Board;
using Board.Tiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Tile debugStartTile;
    public bool showTiles;
    [SerializeField] private GameObject uiSpinner;
    [SerializeField] private GameObject uiSelector;
    [SerializeField] private Transform DEBUG_SpinnerParent;
    [SerializeField] private Transform DEBUG_SelectorParent;

    //May make sense to move this into BoardController class
    [Header("Objects")] 
    [SerializeField] private Transform playerParentObj;
    [SerializeField] private TextMesh diceObj;
    [SerializeField] private Transform shredderObj;
    
    [SerializeField] private GameObject dice;
    [SerializeField] private Vector2 diceSpawnHeightByRadius;

    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => players[curTurn];

    public Transform CameraArm;
    public static GameManager gameManager { get; set; }

    private byte curRound;
    private int curTurn;
    private int diceRemainder;
    
    private BoardPlayer [] players;
    public int DiceRemainder => diceRemainder;

    //Move into board player;
    private bool _inMenu;
    public bool InMenu => _inMenu;

    private void Awake()
    {
        print("GameManagerMade");
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new BoardPlayer[playerParentObj.childCount];
        
        for (int i = 0; i < playerParentObj.childCount; i++)
        {
            players[i] = playerParentObj.GetChild(i).GetComponent<BoardPlayer>();
        }

        StaticHelpers.Shuffle(players);

        diceObj.transform.parent = players[0].transform;
        shredderObj.parent = players[0].transform;

        //_players = new BoardPlayer[players];
        BeginGame();
    }

    public void BeginGame()
    {
        print(GetCurrentPlayer);
        GetCurrentPlayer.currentTile = debugStartTile;

        foreach (BoardPlayer player in players)
        {
            player.transform.position = debugStartTile.transform.position - player.GetHeight;
        }
        //DEBUG
        RollDice();
    }

    private void RollDice()
    {
        Vector3 startLoc = new Vector3(Random.Range(-diceSpawnHeightByRadius.y, diceSpawnHeightByRadius.y),
            Random.Range(-diceSpawnHeightByRadius.x, diceSpawnHeightByRadius.x), 
            Random.Range(0, diceSpawnHeightByRadius.y) + diceSpawnHeightByRadius.y);
        Instantiate(dice, startLoc, Quaternion.identity).GetComponent<DiceScript>().OnCompleted = CheckDice;
    }

    private void CheckDice(int num)
    {
        num = 3;
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
        UpdateCamera();
    }

    void EndTurn()
    {
        //curTurn = (curTurn + 1) % players;
    }

    //This is called after a tile is landed on
    public void EndAction(Tile nextTile, bool costAction)
    {
        if (costAction)
        {
            Debug.Log("End Action: " + (diceRemainder - 1));
            if (diceRemainder-- == 0)
            {
                diceObj.gameObject.SetActive(false);
                EndTurn();
                return;
            }

            diceObj.text = diceRemainder.ToString();

        }
        GetCurrentPlayer.MoveToTile(nextTile);
    }

    //Somewhat complicated generic function. Essentially takes a list of objects and formats them in an appropriate manner./
    //Then the selected object is returned (val) with a state. 
    //If the UI is random, the user isn't actually allowed to select the option, but it should be dynamic for the sake of it's more interesting.
    public void CreateSelectionUI(AwardableEvents[] objects, bool isSpinner, bool shouldShuffle, BoardPlayer ply, int randomItemsToDisplay = 1)
    {
        GameObject go;
        if(shouldShuffle)
            StaticHelpers.Shuffle(objects);
        if (isSpinner)
        {
            //Create UI Spinner
            go = Instantiate(uiSpinner, DEBUG_SpinnerParent);
            SpinnerScript s = go.GetComponent<SpinnerScript>();
            s.Init(objects, ply);
            //return s.Init(objects, spawnedUILoc);
        }
        else
        {
            //Create UI SelectorScript
            go = Instantiate(uiSelector, DEBUG_SelectorParent);
            SelectorScript s = go.GetComponent<SelectorScript>();
            _inMenu = true;
            //Safe cast into items.
            s.Init(objects as Item[], ply, randomItemsToDisplay);
        }
    }

    


    public void LoadMiniGame(string gameName)
    {
        //Complete all tasks BEFORE loading.
        SceneManager.LoadScene(gameName);
    }

}

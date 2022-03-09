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
    [SerializeField] private GameObject uiSpinner;
    [SerializeField] private GameObject uiSelector;
    [SerializeField] private Transform DEBUG_SpinnerParent;
    [SerializeField] private Transform DEBUG_SelectorParent;

    //May make sense to move this into BoardController class
    [SerializeField] private BoardPlayer[] players;

    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => players[curTurn];

    public Transform CameraArm;
    public static GameManager gameManager { get; set; }

    private byte curRound;
    private int curTurn;
    private int diceRemainder;

    //Move into board player;
    private bool _inMenu;
    public bool InMenu => _inMenu;

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
    }

    // Start is called before the first frame update
    void Start()
    {
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
        GetCurrentPlayer.currentTile.LandedOn(GetCurrentPlayer);
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
    public void EndAction(GameObject nextTile, bool costAction)
    {

        if (costAction)
        {
            if (--diceRemainder == 0)
            {
                EndTurn();
            }
        }
    }

    //Somewhat complicated generic function. Essentially takes a list of objects and formats them in an appropriate manner./
    //Then the selected object is returned (val) with a state. 
    //If the UI is random, the user isn't actually allowed to select the option, but it should be dynamic for the sake of it's more interesting.
    public void CreateSelectionUI(AwardableEvents[] objects, bool isSpinner, bool shouldShuffle, BoardPlayer ply, int randomItemsToDisplay = 1)
    {
        GameObject go;
        if(shouldShuffle)
            Shuffle(objects);
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

    //Custom Variation of FisherYates array shuffle method.
    private static void Shuffle <T>(T [] array)
    {
        int length = array.Length;
        while(length > 1)
        {
            //Set RNG to be a random element in the array (Excluding those which have been modified)
            int rng = Random.Range(0, length--);

            //Set temp to be last element
            T temp = array[length];
            //Set last element to be the random selected element
            array[length] = array[rng];
            //Set Random element to be what the old last element was
            array[rng] = temp;
        }
    }


    public void LoadMiniGame(string gameName)
    {
        //Complete all tasks BEFORE loading.
        SceneManager.LoadScene(gameName);
    }

}

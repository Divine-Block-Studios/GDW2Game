using System.Collections.Generic;
using Board.Tiles;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject debugStartTile;
    [SerializeField] private GameObject uiSpinner;
    [SerializeField] private GameObject uiSelector;

    [SerializeField] private Vector2 spawnedUILoc;
    
    //May make sense to move this into BoardController class
    [SerializeField] private BoardPlayer [] players;

    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => players[curTurn];

    public Transform CameraArm;
    public static GameManager gameManager { get; set; }
    
    private byte curRound;
    private int curTurn;
    private int diceRemainder;

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
        print(GetCurrentPlayer);
        debugStartTile.GetComponent<Tile>().LandedOn(GetCurrentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void CreateSelectionUI<T>(List<T>objects, bool isSpinner, out T val)
    {
        GameObject go;
        if (isSpinner)
        {
            //Create UI Spinner
            go = Instantiate(uiSpinner, spawnedUILoc, Quaternion.identity);
            val = objects[Random.Range(0, objects.Count)];
        }
        else
        {
            //Create UI Selector
            go = Instantiate(uiSelector, spawnedUILoc, Quaternion.identity);

            val = objects[0];
        }
    }
}

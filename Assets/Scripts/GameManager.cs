using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private byte curRound;
    private int curTurn;
    private int diceRemainder;

    [SerializeField] private GameObject debugStartTile;
    
    //May make sense to move this into BoardController class
    [SerializeField] private BoardPlayer [] _players;
    
    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => _players[curTurn];

    public Transform CameraArm;
    public static GameManager gameManager { get; set; }

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
    public void EndAction(Tile nextTile, bool costAction)
    {

        if (costAction)
        {
            if (--diceRemainder == 0)
            {
                EndTurn();
            }
        }
    }
}

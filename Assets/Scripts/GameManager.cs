using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Max 20, cus if we expand to PUN we can only handle 20... Alternatively, if we do p2p...
    //[SerializeField] private byte players;

    private byte curRound;
    private int curTurn;
    [SerializeField]
    private BoardPlayer [] _players;
    
    //This is looking like it should be in board manager
    public BoardPlayer GetCurrentPlayer => _players[curTurn];
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndTurn()
    {
        //curTurn = (curTurn + 1) % players;
    }
}

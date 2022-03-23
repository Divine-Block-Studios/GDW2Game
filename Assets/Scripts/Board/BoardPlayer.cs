using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Board.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BoardPlayer : MonoBehaviour
{
    [Header("Stats")] 
    //This should be set in player settings and forwarded through GM?
    public Sprite playerImg;
    [SerializeField] private float moveSpeed;

    [Header("Debug (None of this should be exposed)")]
    public Tile currentTile;

    public ushort coins;
    
    private Item _item;

    private BoardInputControls ctrls;

    public void InMenu(bool val)
    {
        ctrls.canRayCast = !val;
    }

    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            GameManager.gameManager.UpdateUIElements();
        }
    }

    private byte _location;
    private byte _stars;

    private Vector3 imgHeight;

    public Vector3 GetHeight => imgHeight;

    [SerializeField] private TextMeshProUGUI coinsText;

    private void Awake()
    {
        //This is temporary, It should show the coins of the CURRENT player. if the player presses esc, or the "esc" button for IOS they can see all players Icons, balances and names.
        coinsText.text = coins.ToString();
        imgHeight = new Vector3(0, 0,GetComponent<SpriteRenderer>().size.y + 0.15f);
        ctrls = GetComponent<BoardInputControls>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = GameManager.gameManager.CameraArm.rotation;
    }

    public void MoveToTile(Tile moveToTile)
    {
        print("Moving to tile: " + moveToTile);
        Vector3 start = currentTile.transform.position;
        Debug.Log(moveToTile.name);
        Vector3 end = moveToTile.transform.position;
        
        print(imgHeight);
        if (Mathf.Abs(end.z - start.z) > imgHeight.z)
        {
            Debug.Log("Needs to Slerp");//new Vector3(start.x, start.y, end.z - imgHeight.z)
            StaticHelpers.MoveSlerp(transform, start - imgHeight, end - imgHeight,  moveSpeed + 20, 
                () => moveToTile.LandedOn(this));
        }
        else
        {
            Debug.Log("Needs to Lerp: " + moveSpeed);
            StaticHelpers.MoveLerp(transform, start - imgHeight, end - imgHeight, moveSpeed, () => moveToTile.LandedOn(this), true);
        }
        //Move along the direction vector at a set speed.
        //Cast two rays, one along ZX one along Y.
    }

    public void AddCoins(int addAmount)
    {
        coins = (ushort)Mathf.Clamp(coins + addAmount, 0, ushort.MaxValue);
    }

    public void UseItem()
    {
        _item.Init(this);
        _item = null;
        GameManager.gameManager.UpdateUIElements();
    }

    //Sin wave turning?
    //Bouncing Character Up and down... Checkpointed system, Move in arcs...
    
}

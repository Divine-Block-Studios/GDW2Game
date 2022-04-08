using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Board.Tiles;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BoardPlayer : MonoBehaviourPun
{
    [Header("Stats")] 
    //This should be set in player settings and forwarded through GM?
    [HideInInspector]
    public Sprite playerImg;
    [SerializeField] private float moveSpeed;

    [Header("Debug (None of this should be exposed)")]
    public Tile currentTile;

    public ushort coins;// { get; private set; }

    private Item _item;

    private BoardInputControls ctrls;

    private ParticleSystem _particleSystem;
    private SpriteRenderer sr;
    private MeshRenderer mr;

    private Vector3 myPlatform;
    
    [HideInInspector]
    public Vector3 offSet;

    public bool isAlive = true;

    public int plyIndex;

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
        //TODO
        //This is temporary, It should show the coins of the CURRENT player. if the player presses esc, or the "esc" button for IOS they can see all players Icons, balances and names.
        //coinsText.text = coins.ToString();
        if(myPlatform == Vector3.zero)
            myPlatform = transform.position;
        imgHeight = new Vector3(0, 0,GetComponent<SpriteRenderer>().size.y + 0.15f);
        playerImg = GetComponent<SpriteRenderer>().sprite;
        ctrls = GetComponent<BoardInputControls>();
        ctrls.cameraArmBase = GameManager.gameManager.CameraArm;
        sr = transform.GetComponent<SpriteRenderer>();
        _particleSystem = transform.GetChild(1).GetComponent<ParticleSystem>();
        mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        
        offSet.y = GetComponent<RectTransform>().rect.height+ 0.3f;
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
            StaticHelpers.MoveSlerp(transform, start + offSet, end + offSet,  moveSpeed + 20, 
                () => moveToTile.LandedOn(this));
        }
        else
        {
            Debug.Log("Needs to Lerp: " + moveSpeed);
            StaticHelpers.MoveLerp(transform, start + offSet, end + offSet, moveSpeed, () => moveToTile.LandedOn(this), true);
        }
        //Move along the direction vector at a set speed.
        //Cast two rays, one along ZX one along Y.
    }

    public void UpdateCoins(int val)
    {
        photonView.RPC("UpdateCoins", RpcTarget.All, plyIndex, val);
    }

    [PunRPC]
    private void UpdateCoins(int plyerIndex, int val)
    {
        print("Adding coins to: " + GameManager.gameManager.players[plyerIndex].name + " - " + val + " | " + plyerIndex + " I am " + plyIndex + " - " + name + " Expected [ifmine]: " +(GameManager.gameManager.players[plyerIndex].coins + " + " +(ushort)Mathf.Clamp(coins + val, 0, ushort.MaxValue)));
        GameManager.gameManager.players[plyerIndex].coins = (ushort)Mathf.Clamp(coins + val, 0, ushort.MaxValue);
    }

    public void UseItem()
    {
        _item.Init(this);
        photonView.RPC("ModifyItem", RpcTarget.All,null, plyIndex);
    }

    [PunRPC]
    private void ModifyItem(string itemName, int plyerIndex)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            GameManager.gameManager.players[plyerIndex]._item = null;
            GameManager.gameManager.UpdateUIElements();
        }
    }

    public async void Teleport(Vector3 loc, bool includeOffset)
    {
        photonView.RPC("ActivateParticles", RpcTarget.All);
        sr.enabled = false;
        mr.enabled = false;
        await Task.Delay((int) (_particleSystem.main.duration * 1000));

        sr.enabled = true;
        mr.enabled = true;

        Vector3 vec = (includeOffset)?offSet:new Vector3(0,offSet.y,0);
        
        print(vec);
        
        transform.position = loc + vec;
    }

    [PunRPC]
    private void ActivateParticles()
    {
        _particleSystem.Play();
    }
    //Sin wave turning?
    //Bouncing Character Up and down... Checkpointed system, Move in arcs...
}

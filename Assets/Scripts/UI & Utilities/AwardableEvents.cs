using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AwardableEvents : ScriptableObject
{
    public Sprite icon;
    public string awardName;
    
    [SerializeField] private int cost;
    public int Cost => cost;
    //If true, when the item is awarded via gamble tile, it will not replace the users current item, and activate immediately.
    //May be unbalanced
    public bool instantlyUsed;
    public abstract void Init(BoardPlayer ply);
}

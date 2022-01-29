using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AwardableEvents : ScriptableObject
{
    public Sprite icon;
    public string awardName;
    public abstract void Init(BoardPlayer ply);
}

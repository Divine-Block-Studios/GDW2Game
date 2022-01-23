using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Used in shops or other.
    protected bool _forcePlayerInteraction;
    
    //If the player uses this tile, should it take a move?
    protected bool _costsMoveToPass = true;

    //Next tile by default. If the tile moves in multiple directions, this is still important.
    [SerializeField] protected Tile nextTile;

    public virtual void LandedOn(BoardPlayer player)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTile : Tile
{
    [SerializeField] private bool giveCoins;
    [SerializeField] private ushort amount;
    public override void LandedOn(BoardPlayer player)
    {
        if (giveCoins)
        {
            player.Coins -= amount;
        }
        else
        {
        }

    }
}

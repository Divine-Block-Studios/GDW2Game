using System;
using UnityEngine;

namespace Board.Tiles
{
    public class CoinTile : Tile
    {
        [SerializeField] private bool giveCoins;
        [SerializeField] private ushort amount;

        private void Start()
        {
            _costsMoveToPass = true;
        }

        public override void LandedOn(BoardPlayer player)
        {
            if (giveCoins)
            {
                player.coins += amount;
            }
            else
            {
                player.coins -= amount;
            }
            base.LandedOn(player);
        }
    }
}

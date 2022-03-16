using System;
using UnityEngine;

namespace Board.Tiles
{
    public class CoinTile : Tile
    {
        [SerializeField] private short amount;

        private void Start()
        {
            _costsMoveToPass = true;
        }

        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            player.AddCoins(amount);
        }
    }
}

using UnityEngine;

namespace Board.Tiles
{
    public class CoinTile : Tile
    {
        [SerializeField] private bool giveCoins;
        [SerializeField] private ushort amount;
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
            GameManager.gameManager.EndAction(nextTile, _costsMoveToPass);
        }
    }
}

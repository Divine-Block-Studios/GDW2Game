using UnityEngine;

namespace Board.Tiles
{
    public class ChanceTile : Tile
    {
        //[SerializeField] private 
        public override void LandedOn(BoardPlayer player)
        {
            base.LandedOn(player);
        }
    }
}

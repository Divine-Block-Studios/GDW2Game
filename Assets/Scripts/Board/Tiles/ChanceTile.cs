using UnityEngine;

namespace Board.Tiles
{
    public class ChanceTile : Tile
    {
        //[SerializeField] private 
        private void Start()
        {
            _costsMoveToPass = true;
            _forcePlayerInteraction = false;
        }
        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            
        }
    }
}

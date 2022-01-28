using UnityEngine;

namespace Board.Tiles
{
    public class Tile : MonoBehaviour
    {
        //Used in shops or other.
        protected bool _forcePlayerInteraction;
    
        //If the player uses this tile, should it take a move?
        protected bool _costsMoveToPass;

        //Next tile by default. If the tile moves in multiple directions, this is still important.
        [SerializeField] protected GameObject nextTile;

        public virtual void LandedOn(BoardPlayer player)
        {
            GameManager.gameManager.EndAction(nextTile, _costsMoveToPass);
        }
    }
}

using UnityEngine;

namespace Board.Tiles
{
    public class WarpTile : Tile
    {
        //A random one of these is picked.
        [SerializeField] private Tile [] warpToTile;
        
        // Start is called before the first frame update
        void Awake()
        {
            _forcePlayerInteraction = true;
            _costsMoveToPass = true;
        }

        public override void LandedOn(BoardPlayer player)
        {
            //Create a UI for yes or no that takes gold
            Debug.Log("Player has landed on: " + gameObject.name);
            if (true)
            {
                
            }
            else
            {
                GameManager.gameManager.EndAction(nextTile, _costsMoveToPass);
            }
        }
    }
}

using System;
using UnityEngine;

namespace Board.Tiles
{
    public class Tile : MonoBehaviour
    {
        //Used in shops or other.
        protected bool _forcePlayerInteraction;

        //If the player uses this tile, should it take a move?
        protected bool _costsMoveToPass;

        public bool CostsMoveToPass => _costsMoveToPass;

        private readonly Vector3[] _locations = new Vector3[6];
        public Vector3 GetStandingLoc(int i) => _locations[i];

        //Next tile by default. If the tile moves in multiple directions, this is still important.
        [SerializeField] protected Tile nextTile;

        public Tile NextTile => nextTile;

        private void Awake()
        {
            print("AwakeMain: " + gameObject.name);

            if (GameManager.gameManager)
            {
                print("True");
            }


            if(GameManager.gameManager.showTiles)
                GetComponent<MeshRenderer>().enabled = false;
            float dims = transform.localScale.x / 2;

            for (int i = 0; i < _locations.Length; i++)
            {
                float rotation = (60 * i) - 30;
                Vector3 vertex = new Vector3(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad));
                _locations[i] = vertex * dims;
            }
            print("AwakeMain: " + gameObject.name);
        }

        public void LandedOn(BoardPlayer player)
        {
            //Set this to be the current tile
            player.currentTile = this;
            //if force interaction
            Debug.Log("Testing: " + _forcePlayerInteraction + " - "  + _costsMoveToPass + " - " + gameObject.name + " - " + GameManager.gameManager.DiceRemainder);
            if (_forcePlayerInteraction)
            {
                //Call function
                LandedOnFunctionality(player);
                return;
            }
            if (GameManager.gameManager.DiceRemainder == 0)
            {
                //if remaining moves is 0, call functionality regardless.
                LandedOnFunctionality(player);
            }
            GameManager.gameManager.EndAction(nextTile, _costsMoveToPass);
            //End turn regardless of outcome.
            
        }

        protected virtual void LandedOnFunctionality(BoardPlayer player)
        {
            
        }

        public virtual void OnPressed(BoardPlayer player)
        {
            //if(player.IsTurn && player.Tile == WarpTile && Warptile.contains(this)? ) ()
            //player.MoveToTile(this);
            //else{} (Shake tile anim?)
        }
    }
}

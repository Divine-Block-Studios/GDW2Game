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

        private readonly Vector3[] _locations = new Vector3[6];
        public Vector3 GetStandingLoc(int i) => _locations[i];

        //Next tile by default. If the tile moves in multiple directions, this is still important.
        [SerializeField] protected Tile nextTile;

        public Tile NextTile => nextTile;

        private void Awake()
        {
            GetComponent<MeshRenderer>().enabled = false;
            float dims = transform.localScale.x / 2;

            for (int i = 0; i < _locations.Length; i++)
            {
                float rotation = (60 * i) - 30;
                Vector3 vertex = new Vector3(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad));
                _locations[i] = vertex * dims;
                Debug.Log(i+ " - " +vertex);
                Debug.Log(_locations[i]);
            }

        }

        public virtual void LandedOn(BoardPlayer player)
        {
            player.currentTile = this;
            GameManager.gameManager.EndAction(nextTile, _costsMoveToPass);
        }

        public virtual void OnPressed(BoardPlayer player)
        {
            //if(player.IsTurn && player.Tile == WarpTile && Warptile.contains(this)? ) ()
            player.MoveToTile(this);
            //else{} (Shake tile anim?)
        }
    }
}

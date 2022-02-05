using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Board.Tiles
{
    public class GambleTile : Tile
    {
        [SerializeField] private AwardableEvents[] miniGames;
        [SerializeField] private bool shouldShuffle;
        public override void LandedOn(BoardPlayer player)
        {
            GameManager.gameManager.CreateSelectionUI(miniGames, true, shouldShuffle, player);
            //Do not end the turn. This will be handled outside of this script. Sorry for rabbit holeing couldn't get it working otherwise
        }
    }
}

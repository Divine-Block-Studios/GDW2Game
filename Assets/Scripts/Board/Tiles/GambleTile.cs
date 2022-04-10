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

        private void Start()
        {
            _costsMoveToPass = true;
            _forcePlayerInteraction = false;
        }

        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            GameManager.gameManager.CreateSelectionUI(miniGames, true, shouldShuffle, player, 0, () => GameManager.gameManager.EndTurn());
            //Do not end the turn. This will be handled outside of this script. Sorry for rabbit holeing couldn't get it working otherwise
        }
    }
}

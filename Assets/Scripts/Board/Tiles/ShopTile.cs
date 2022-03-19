using System;
using UnityEngine;

namespace Board.Tiles
{
    public class ShopTile : Tile
    {
        [SerializeField] private AwardableEvents[] purchaseAbleItems;

        [SerializeField] private bool shouldShuffle;

        [SerializeField] private int randomItemsToDisplay;
        
        private void Start()
        {
            _costsMoveToPass = true;
            _forcePlayerInteraction = true;
        }
        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            //Can cause run time exception... Keep track of this. (Sending Item[] instead of AwardableEvent)
            
            //If no items, or set to display no items
            if (purchaseAbleItems.Length == 0)
            {
                throw new Exception("Shop was not created properly.");
            }
            randomItemsToDisplay = Mathf.Clamp(randomItemsToDisplay, 1, purchaseAbleItems.Length);

            GameManager.gameManager.CreateSelectionUI(purchaseAbleItems, false, shouldShuffle, player, randomItemsToDisplay);
        }
    }
}

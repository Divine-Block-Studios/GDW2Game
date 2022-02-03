using UnityEngine;

namespace Board.Tiles
{
    public class ShopTile : Tile
    {
        [SerializeField] private Item[] purchaseAbleItems;
        public override void LandedOn(BoardPlayer player)
        {
            //Can cause run time exception... Keep track of this. (Sending Item[] instead of AwardableEvent)
            GameManager.gameManager.CreateSelectionUI(purchaseAbleItems, false, player);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Board.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScript : MonoBehaviour
{
    [SerializeField] private Transform itemBacking;
    [SerializeField] private GameObject itemTemplate;
    [SerializeField] private GameObject scrollBar;
    [SerializeField] private Sprite unavailableBackground;
    private float _boxDeltaY;
    private bool _inShop;

    private Action post;

    //TODO: Add the ability to press on an item ICON and have it display what it does.
    //It would have to be a special script that get's applied to all items that adds a button that on press displays what the item does.

    public void Init(AwardableEvents[] items, BoardPlayer ply, int randomItemsToDisplay, Action onComplete = null)
    {
        ply.InMenu(true);
        print("init shop");
        _inShop = true;
        post = onComplete;
        List<GameObject> gos = new List<GameObject>();
        float sizeDeltaY = itemTemplate.GetComponent<RectTransform>().sizeDelta.y;
        
        //if distance between items * num of items is larger than what would show
        if ((sizeDeltaY + 5) * randomItemsToDisplay > itemBacking.GetComponent<RectTransform>().sizeDelta.y)
        {
            print("Showing Scroll");
            _boxDeltaY = (sizeDeltaY + 5) * (randomItemsToDisplay-3);
            Scrollbar sb = scrollBar.GetComponent<Scrollbar>();
            sb.onValueChanged.AddListener(ShiftItems);
            scrollBar.SetActive(true);
        }
        else
        {
            print("Hiding Scroll");
            scrollBar.SetActive(false);
        }

        for (int i = 0; i < randomItemsToDisplay; i++)
        {
            //Instantiate the item template in the correct position, With the parent of item backing and grab the button Component
            gos.Add(Instantiate(itemTemplate, itemBacking));
            gos[i].transform.localPosition = new Vector3(itemTemplate.transform.localPosition.x, itemTemplate.transform.localPosition.y - (sizeDeltaY + 5) * i, 0);

            gos[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].icon;
            gos[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = items[i].awardName;
            gos[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = items[i].Cost.ToString();
            
            print("debug: "+ ply.Item + " - " + items[i] + (ply.coins < items[i].Cost));
            if (ply.Item == items[i] || ply.coins < items[i].Cost)
            {
                
                //Not the best place to call this, but it's already weird cus it skips the first obj.
                gos[i].GetComponent<Image>().sprite = unavailableBackground;
                gos[i].transform.GetChild(4).gameObject.SetActive(true);
                continue;
            }
            
            int temp = i;
            print("Alpha: " + i +" - " + randomItemsToDisplay);
            gos[i].GetComponent<Button>().onClick.AddListener(() => BuyItem(ply, items[temp]));
            //gos[i].transform.GetChild(3).GetComponent<Image>().sprite = items[i].icon; Currency Icon

            //Temp is required in order to work.
            
            //If player doesn't have money for any item, kick them out. OR if they have max items, kick them out.
            /*if ()
            {
            
            }*/
        }
    }
    public void CloseShop()
    {
        
        //Continue game process
        print("Closing Shop.");
        BoardPlayer ply = GameManager.gameManager.GetCurrentPlayer;
        ply.InMenu(false);
        Tile tile = ply.currentTile;
        if (ply.Item)
        {
            if (ply.Item.instantlyUsed)
            {
                ply.Item.Init(ply);
            }
        }
        GameManager.gameManager.EndAction(tile.NextTile, tile.CostsMoveToPass);
        post?.Invoke();
        Destroy(gameObject);
    }

    private void BuyItem(BoardPlayer ply, AwardableEvents item)
    {
        print("Player attempting to buy: " + item.awardName + " for: " + item.Cost + " ... ");
        if (item is Item it)
        {
            if (item.instantlyUsed)
            {
                it.Init(ply);
            }
            else
            {
                ply.Item = it;
            }
        }
        else
        {
            //You cannot hold a minigame.
            item.Init(ply);
        }
        ply.AddCoins(-item.Cost);
        CloseShop();
        GameManager.gameManager.UpdateUIElements();
    }

    private void ShiftItems(float dir)
    {
        itemBacking.localPosition = new Vector3(0, _boxDeltaY * dir, 0);
    }
}

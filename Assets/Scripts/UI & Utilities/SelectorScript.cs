using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SelectorScript : MonoBehaviour
{
    [SerializeField] private Transform itemBacking;
    [SerializeField] private GameObject itemTemplate;
    [SerializeField] private GameObject scrollBar;
    [SerializeField] private int outroDelayMS;
    private float _boxDeltaY;
    private bool _inShop;

    //TODO: Add the ability to press on an item ICON and have it display what it does.
    //It would have to be a special script that get's applied to all items that adds a button that on press displays what the item does.

    public void Init(Item[] items, BoardPlayer ply, int randomItemsToDisplay)
    {
        print("init shop");
        _inShop = true;

        List<GameObject> gos = new List<GameObject> {itemTemplate};
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
            //First item is already in place.
            if (i > 0)
            {
                //Instantiate the item template in the correct position, With the parent of item backing and grab the button Component
                gos.Add(Instantiate(itemTemplate, itemBacking));
                gos[i].transform.localPosition = new Vector3(itemTemplate.transform.localPosition.x, itemTemplate.transform.localPosition.y - (sizeDeltaY + 5) * i, 0);
                
            }
            gos[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].icon;
            gos[i].transform.GetChild(1).GetComponent<Text>().text = items[i].name;
            gos[i].transform.GetChild(2).GetComponent<Text>().text = items[i].Cost.ToString();
            //gos[i].transform.GetChild(3).GetComponent<Image>().sprite = items[i].icon; Currency Icon

            //Temp is required in order to work.
            int temp = i;
            gos[i].GetComponent<Button>().onClick.AddListener(() => BuyItem(ply, items[temp]));
            //If player doesn't have money for any item, kick them out. OR if they have max items, kick them out.
            /*if ()
            {
            
            }*/
        }

    }
    
    private async void InShop(int a, int b)
    {
        while (_inShop)
        {
            await Task.Delay(1);
        }
        //Show Outro card.
        //await Task.Delay(outroDelayMS);

        
    }

    public void CloseShop()
    {
        //Continue game process
        print("Closing Shop.");
    }

    private void BuyItem(BoardPlayer ply, Item item)
    {
        print("Player attempting to buy: " + item.name + " for: " + item.Cost + " ... ");
        /*
        if () // Player Can buy Item
        {
            
        }
        else // Player Cannot Buy Item
        {
            _inShop = false;
        }*/
    }

    private void ShiftItems(float dir)
    {
        itemBacking.localPosition = new Vector3(0, _boxDeltaY * dir, 0);
        print(itemBacking.localPosition);
    }
}

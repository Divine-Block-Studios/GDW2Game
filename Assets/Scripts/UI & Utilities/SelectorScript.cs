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
    [SerializeField] private int randomItemsToDisplay;
    [SerializeField] private int outroDelayMS;

    private bool _inShop;

    //TODO: Add the ability to press on an item ICON and have it display what it does.
    //It would have to be a special script that get's applied to all items that adds a button that on press displays what the item does.

    public void Init(Item[] items, BoardPlayer ply)
    {

        //If no items, or set to display no items
        if (items.Length == 0)
        {
            throw new Exception("Shop was not created properly.");
        }
        randomItemsToDisplay = Mathf.Clamp(randomItemsToDisplay, 1, items.Length);
        

        print("init shop");
        _inShop = true;
        items = RandomizeArray(items);

        List<GameObject> gos = new List<GameObject> {itemTemplate};
        float sizeDeltaY = itemTemplate.GetComponent<RectTransform>().sizeDelta.y;
        float boxDeltaY = itemBacking.GetComponent<RectTransform>().sizeDelta.y;
        
        //if distance between items * num of items is larger than what would show
        if ((sizeDeltaY + 5) * randomItemsToDisplay - 5 > itemBacking.GetComponent<RectTransform>().sizeDelta.y)
        {
            print("Showing Scroll");
            Scrollbar sb = scrollBar.GetComponent<Scrollbar>();
            sb.size = (sizeDeltaY + 5) * randomItemsToDisplay - 5;
            sb.onValueChanged.AddListener((dir) => ShiftItems(gos, dir));
            scrollBar.SetActive(true);
        }
        else
        {
            print("Hiding Scroll");
            scrollBar.SetActive(false);
        }

        for (int i = 0; i < randomItemsToDisplay; i++)
        {
            print("iteration: " + i + " - " + sizeDeltaY);
            //First item is already in place.
            if (i > 0)
            {
                //Instantiate the item template in the correct position, With the parent of item backing and grab the button Component
                gos.Add(Instantiate(itemTemplate, itemBacking));
                gos[i].transform.localPosition = new Vector3(itemTemplate.transform.localPosition.x, itemTemplate.transform.localPosition.y - (sizeDeltaY + 5) * i, 0);
            }

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

    private void ShiftItems(List<GameObject> gos, float dir)
    {
        print("Trying to Shift Items");
        Vector3 move = new Vector3(0, dir);
        foreach (GameObject go in gos)
        {
            go.transform.position += move;
        }
    }

    private Item [] RandomizeArray(Item [] items)
    {
        int len = items.Length;
        while (len > 1)
        {
            int rng = Random.Range(0, len--);
            Item temp = items[rng];
            items[len] = items[rng];
            items[rng] = temp;
        }

        return items;
    }
}

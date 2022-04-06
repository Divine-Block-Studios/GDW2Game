using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScoreBoard : MonoBehaviour
{
    [SerializeField] private Sprite openImg;
    [SerializeField] private Sprite closeImg;
    [SerializeField] private float moveX;
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerBadge;
    [SerializeField] private Vector3[] badgePositions;


    private Image _openButton;
    private List<GameObject> _badges;
    
    // Start is called before the first frame update
    private void Start()
    {
        _openButton = transform.GetChild(0).GetComponent<Image>();

        _badges = GameObject.FindGameObjectsWithTag("Player").ToList();

        for (int i = 0; i  < _badges.Count; i++)
        {
            _badges[i] = Instantiate(playerBadge, badgePositions[i], Quaternion.identity);
            UpdateScoreBoard();
        }


        string[] test = {"0", "8", "2"};
        double[] ints = {0, 8, 2};
        
        StaticHelpers.Sort(ref test, ints,true);
        
        print(test[0] + " - " + test[1] + " - " + test[2]);

    }

    public async void Toggle()
    {
        moveX *= -1;
        Vector3 newPos = transform.position + new Vector3(moveX, 0, 0);
        StaticHelpers.MoveLerp(transform, transform.position, newPos, speed, () =>
        {
            _openButton.sprite = moveX < 0 ? closeImg : openImg;
        });
    }

    public void UpdateScoreBoard()
    {
        BoardPlayer[] plysSorted = GameManager.gameManager.players;
        double[] playerScores = new double[plysSorted.Length];

        for (int i = 0; i < plysSorted.Length; i++)
        {
            playerScores[i] = plysSorted[i].coins;
        }
        
        StaticHelpers.Sort(ref plysSorted, playerScores,true);

        for (int i = 0; i < _badges.Count; i++)
        {
            Transform t = _badges[i].transform;
            t.gameObject.name = plysSorted[i].gameObject.name;
            //Rank
            t.GetChild(0).GetComponent<TextMeshProUGUI>().text = i +".";
            //Icon
            t.GetChild(1).GetComponent<Image>().sprite = plysSorted[i].playerImg;
            //Player name
            t.GetChild(2).GetComponent<TextMeshProUGUI>().text = t.gameObject.name;
            //Coins
            t.GetChild(3).GetComponent<TextMeshProUGUI>().text = plysSorted[i].coins.ToString();

            if (!plysSorted[i].isAlive)
            { 
                t.GetComponent<Image>().color = new Color(247,121,134, 122);
                t.GetChild(4).gameObject.SetActive(true);
                _badges.RemoveAt(i);
            }
        }
    }
}

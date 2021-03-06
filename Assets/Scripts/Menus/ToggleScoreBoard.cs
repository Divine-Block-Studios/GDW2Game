using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScoreBoard : MonoBehaviour
{
    [SerializeField] private Sprite openImg;
    [SerializeField] private Sprite closeImg;
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerBadge;
    [SerializeField] private Vector3[] badgePositions;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform pt0;
    [SerializeField] private Transform pt1;
    private float maxX;
    private float minX;


    private float moveX;
    private Image _openButton;
    private List<GameObject> _badges;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _openButton = transform.GetChild(0).GetComponent<Image>();

        _badges = new List<GameObject>();

        for (int i = 0; i  < GameManager.gameManager.players.Length; i++)
        {
            _badges.Add(Instantiate(playerBadge, parent));
            _badges[i].transform.localPosition = badgePositions[i];
        }
        moveX = Screen.width/2 + Screen.width/8;
        minX = transform.position.x;
        maxX = minX - moveX;
    }

    public void Toggle()
    {
        moveX *= -1;
        Vector3 newPos = moveX > 0 ?  pt0.position : pt1.position;
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
        
        StaticHelpers.Sort(ref plysSorted, playerScores,false);
        
        for (int i = 0; i < _badges.Count; i++)
        {
            Transform t = _badges[i].transform;
            t.gameObject.name = plysSorted[i].gameObject.name;
            //Rank
            t.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i+1) +".";
            //Icon
            t.GetChild(1).GetComponent<Image>().sprite = plysSorted[i].playerImg;
            //Player name
            t.GetChild(2).GetComponent<TextMeshProUGUI>().text = t.gameObject.name;
            //Coins
            t.GetChild(3).GetComponent<TextMeshProUGUI>().text = plysSorted[i].coins.ToString();

            if (plysSorted[i].Item)
            {
                t.GetChild(4).GetComponent<Image>().sprite = plysSorted[i].Item.icon;
                t.GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                t.GetChild(4).gameObject.SetActive(false);
            }

            if (!plysSorted[i].isAlive)
            { 
                t.GetComponent<Image>().color = new Color(247,121,134, 122);
                t.GetChild(6).gameObject.SetActive(true);
                _badges.RemoveAt(i);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CheeseSamurai : MonoBehaviourPun
{
    [Header("Cheese Control")]
    [SerializeField] private Transform throwA;
    [SerializeField] private Transform throwB;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] cheeses;

    [Header("Cheese Elements")]
    [SerializeField] private GameObject shinyParticles;
    [SerializeField] private GameObject stinkyParticles;
    [Range(0,100)]
    [SerializeField] private float shinyChance;
    [Range(0,100)]
    [SerializeField] private float stinkyChance;
    [SerializeField] private int shinyMultiplier;
    [SerializeField] private int stinkyMultiplier;
    
    [Header("Canvas Elements")]
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private Transform myScore;
    public TextMeshProUGUI myScoreText;
    private Image myImage;
    [SerializeField] private Transform[] scoreBoard;
    private TextMeshProUGUI[] scoreBoardTxts;
    private Image []  scoreBoardImgs;
    [SerializeField] private Transform imWinningEffect;
    [SerializeField] private Sprite[] defaultSprites;

    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject endgameCanvas;
    
    [Header("GameSettings")]
    [SerializeField] private float minTimeToCheese;
    [SerializeField] private float maxTimeToCheese;
    [Range(0, 100)] [SerializeField] private float crazyCheeseChance;
    [SerializeField] private int maxCheeseToSpawnWhenCrazy;
    [SerializeField] private int maxCheeseToSpawnDef;
    [SerializeField] private float gameDuration;

    [SerializeField] private float forceScalar = 0.5f;

    private CheesePlayerDatas [] playerdatas;
    private CheesePlayerDatas[] sortedDatas;

    private bool gameComplete;
    private bool done;
    private float delTimer;

    // Start is called before the first frame update
    public void SyncedStart()
    {
        shinyChance /= 100;
        stinkyChance /= 100;
        crazyCheeseChance /= 100;
        
        scoreBoardTxts = new TextMeshProUGUI[scoreBoard.Length];
        scoreBoardImgs = new Image[scoreBoard.Length];
        myImage = myScore.GetChild(0).GetComponent<Image>();
        myScoreText = myScore.GetChild(1).GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < scoreBoard.Length; i++)
        {
            scoreBoardImgs[i] = scoreBoard[i].GetChild(0).GetComponent<Image>();
            scoreBoardTxts[i] = scoreBoard[i].GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        StartGame();
    }

    private void Update()
    {
        if (gameComplete && transform.childCount == 0)
        {
            if (!done)
            {
                done = true;
                photonView.RPC("ImDone", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber - 1);
            }
            delTimer += Time.deltaTime;
            if (delTimer < 10f)
            {
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(0);
            }
        }
    }

    [PunRPC]
    private void ImDone(int playerIndex)
    {
        playerdatas[playerIndex].isDone = true;

        for (int i = 0; i < playerdatas.Length; i++)
        {
            if (!playerdatas[playerIndex].isDone)
            {
                return;
            }
        }
        //Doesn't need to be an RPC because all players should be synced.
        EndGame();
    }

    private async void EndGame()
    {
        float curTime = 0;
        while (curTime < 2)
        {
            curTime += Time.deltaTime;
            await Task.Yield();
        }
        gameplayCanvas.SetActive(false);
        endgameCanvas.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            if (i >= sortedDatas.Length)
            {
                endgameCanvas.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            Transform t = endgameCanvas.transform.GetChild(i);
            t.GetChild(0).GetComponent<Image>().sprite = sortedDatas[i].image;
            t.GetChild(1).GetComponent<TextMeshProUGUI>().text = sortedDatas[i].points.ToString();
        }

    }

    private async void StartGame()
    {
        float curTime = 0;
        float lastCheese = 0;
        float nextCheese = Random.Range(minTimeToCheese, maxTimeToCheese);
        playerdatas = new CheesePlayerDatas[PhotonNetwork.CurrentRoom.PlayerCount];
        sortedDatas = new CheesePlayerDatas[PhotonNetwork.CurrentRoom.PlayerCount];
        if (GameManager.gameManager)
        {
            for (int i = 0; i < playerdatas.Length; i++)
            {
                playerdatas[i] = new CheesePlayerDatas(0, GameManager.gameManager._playerDatas[i].img);
                if (i < 3)
                {
                    scoreBoardTxts[i].text = "0";
                    scoreBoardImgs[i].sprite = playerdatas[i].image;
                }
            }
            for(int x = 2; x >= playerdatas.Length; x++)
            {
                scoreBoard[x].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < playerdatas.Length; i++)
            {
                playerdatas[i] = new CheesePlayerDatas(0,  defaultSprites[i]);
                print("Debug " + PhotonNetwork.CurrentRoom.Players[i + 1].ActorNumber);
                if (i < 3)
                {
                    scoreBoardTxts[i].text = "0";
                    scoreBoardImgs[i].sprite = defaultSprites[i];
                }
            }
            for(int x = 2; x >= playerdatas.Length; x--)
            {
                scoreBoard[x].gameObject.SetActive(false);
            }

            FindObjectOfType<SamuraiController>().playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            myImage.sprite = playerdatas[PhotonNetwork.LocalPlayer.ActorNumber-1].image;
            myScoreText.text = "0";
        }

        while (curTime < gameDuration && Application.isPlaying)
        {
            timer.text = ((int)(gameDuration - curTime)).ToString();
            curTime += Time.deltaTime;

            //If is master
            lastCheese += Time.deltaTime;
            
            if (PhotonNetwork.LocalPlayer.IsMasterClient && lastCheese > nextCheese)
            {
                int iterations;
                if (curTime > 5 && Random.Range(0f, 1f) < crazyCheeseChance)
                {
                    iterations = Random.Range(maxCheeseToSpawnDef, maxCheeseToSpawnWhenCrazy+1);
                }
                else
                {
                    iterations = Random.Range(1, maxCheeseToSpawnDef + 1);
                }

                while (iterations-- > 0)
                {
                    float rngType = Random.Range(0f, 1f);
                    int rngSpawnPoint = Random.Range(0, spawnPoints.Length);
                    int cheeseType = Random.Range(0, cheeses.Length);
                    
                    Vector3 pos = spawnPoints[rngSpawnPoint].position;
                    Vector3 position = throwA.position;
                    Vector3 position1 = throwB.position;
                    float throwPosX = Random.Range(position.x, position1.x);
                    float throwPosY = Random.Range(position.y, position1.y);
                    //GameManager.gameManager.photonView.RPC("SpawnCheese", RpcTarget.All, pos.x, pos.y, pos.z, cheeseType, rngType, throwPosX, throwPosY);

                    print("DEBUG SPAWNING CHEESE");
                    photonView.RPC("SpawnCheese", RpcTarget.All, pos.x, pos.y, pos.z, cheeseType, rngType, throwPosX, throwPosY);
                }
                nextCheese = Random.Range(minTimeToCheese, maxTimeToCheese);
                lastCheese = 0;
            }
            await Task.Yield();
        }
        gameComplete = true;
    }

    [PunRPC]
    private void SpawnCheese(float x, float y, float z, int cheeseNum, float rng, float throwX, float throwY)
    {
        Vector3 pos = new Vector3(x, y, z);
        
        Cheese cheese = Instantiate(cheeses[cheeseNum], pos, cheeses[cheeseNum].transform.rotation, transform).GetComponent<Cheese>();
        
        //Is stinky
        if (rng < stinkyChance)
        {
            cheese.value *= stinkyMultiplier;
            Instantiate(stinkyParticles, cheese.transform);
        }
        //Is Shiny
        else if (rng < shinyChance + stinkyChance)
        {
            cheese.value *= shinyMultiplier;
            Instantiate(shinyParticles, cheese.transform);
        }
        
        Vector2 throwAt = new Vector2(x, y) - new Vector2(throwX, throwY);
        cheese.GetComponent<Rigidbody>().AddForce(-throwAt * forceScalar, ForceMode.Impulse);
    }

    [PunRPC]
    private void UpdateScoreBoard(int player, int value)
    {
        print("Updating player: " + player);
        playerdatas[player].points = value;
        int[] points = new int[playerdatas.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = playerdatas[i].points;
        }
        sortedDatas  =  (CheesePlayerDatas[]) playerdatas.Clone();
        StaticHelpers.Sort(ref sortedDatas, points, false);
        
        for(int x = 0; x < scoreBoard.Length && x < sortedDatas.Length; x++)
        {
            scoreBoardImgs[x].sprite = sortedDatas[x].image;
            scoreBoardTxts[x].text = sortedDatas[x].points.ToString();
        }
    }

    public void UpdateScore(int playerIndex, int curScore)
    {
        photonView.RPC("UpdateScoreBoard", RpcTarget.All, playerIndex, curScore);
    }
}

public class CheesePlayerDatas
{
    public int points;
    public Sprite image;
    public bool isDone;

    public CheesePlayerDatas(int points, Sprite image)
    {
        this.points = points;
        this.image = image;
        isDone = false;
    }
}

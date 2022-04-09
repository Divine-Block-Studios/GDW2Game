using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class CheeseSamurai : MonoBehaviour
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
    [SerializeField] private Transform timer;
    [SerializeField] private Transform myScore;
    [SerializeField] private Transform[] scoreBoard;
    [SerializeField] private Transform imWinningEffect;
    
    [Header("GameSettings")]
    [SerializeField] private float minTimeToCheese;
    [SerializeField] private float maxTimeToCheese;
    [Range(0, 100)] [SerializeField] private float crazyCheeseChance;
    [SerializeField] private int maxCheeseToSpawnWhenCrazy;
    [SerializeField] private int maxCheeseToSpawnDef;
    [SerializeField] private float gameDuration;

    [SerializeField] private float forceScalar = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        shinyChance /= 100;
        stinkyChance /= 100;
        crazyCheeseChance /= 100;
        StartGame();
    }

    [PunRPC]
    private async void StartGame()
    {
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }*/

        float curTime = 0;
        float lastCheese = 0;
        float nextCheese = Random.Range(minTimeToCheese, maxTimeToCheese);
        while (curTime < gameDuration)
        {
            curTime += Time.deltaTime;
            //If is master
            lastCheese += Time.deltaTime;
            if (lastCheese > nextCheese)
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
                    SpawnCheese(pos.x, pos.y, pos.z, cheeseType, rngType, throwPosX, throwPosY);
                }
                nextCheese = Random.Range(minTimeToCheese, maxTimeToCheese);
                lastCheese = 0;
            }

            await Task.Yield();

        }
    }

    [PunRPC]
    private void SpawnCheese(float x, float y, float z, int cheeseNum, float rng, float throwX, float throwY)
    {
        Vector3 pos = new Vector3(x, y, z);
        
        Cheese cheese = Instantiate(cheeses[cheeseNum], pos, Quaternion.identity, transform).GetComponent<Cheese>();
        
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DiceScript : MonoBehaviour
{
    private Rigidbody rb;
    public Action<int> OnCompleted;

    // Update is called once per frame

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(Vector3.zero);
        Vector3 throwDest = GameManager.gameManager.GetCurrentPlayer.transform.position;
        print(throwDest);
        Debug.DrawRay(transform.position, throwDest, Color.magenta, 20f);
        //Debug.DrawRay(transform.position, new Vector3(100,100,100), Color.black, 20f);
        Debug.Log("Hello");
        
        //1) Start high
        //2) Aim straight down, then rotate at a random angle, w/ 2D rotation matrix along two points on a plane.
        //3) throw from A to B.

        float rot = Random.Range(0, 360);
        
        throwDest.x = throwDest.x * Mathf.Cos(rot * Mathf.Deg2Rad)  - throwDest.x * Mathf.Sin(rot * Mathf.Deg2Rad);
        throwDest.z = throwDest.z * Mathf.Sin(rot* Mathf.Deg2Rad)  + throwDest.z * Mathf.Cos(rot* Mathf.Deg2Rad);
        
        print(transform.position + " - " + throwDest);
        
        Debug.DrawRay(transform.position, throwDest, Color.green, 5f, false);
        
        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-5,6),Random.Range(-5,6),Random.Range(-5,6)));
        
        StaticHelpers.ThrowAt(transform, transform.position, throwDest, 10, 50, 0.3f);
        
        TakingTooLong();
    }

    void Update()
    {
        if (rb.IsSleeping())
        {
            OnCompleted.Invoke(CheckDie());
            PhotonNetwork.Destroy(gameObject);
        }
    }
    
    int CheckDie()
    {
        int len = transform.childCount;
        float curLow = 0;
        int val = 0;
        for (int i = 0; i < len; i++)
        {
            float loc = transform.GetChild(i).position.y;
            if (loc > curLow)
            {
                curLow = loc;
                val =  i + 1;
            }
        }
        print("Landed on: " +val);
        return val;
    }


    private async void TakingTooLong()
    {
        await Task.Delay(10000);
        rb.Sleep();
    }
}

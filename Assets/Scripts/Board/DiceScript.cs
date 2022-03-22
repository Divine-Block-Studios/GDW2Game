using System;
using System.Collections;
using System.Collections.Generic;
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
        StaticHelpers.ThrowAt(transform, transform.position, throwDest, 5, 2, 0.1f);
    }

    void Update()
    {
        if (rb.IsSleeping())
        {
            OnCompleted.Invoke(CheckDie());
            Destroy(gameObject);
        }
    }
    
    int CheckDie()
    {
        int len = transform.childCount;
        float curLow = 1;
        int val = 0;
        for (int i = 0; i < len; i++)
        {
            float loc = transform.GetChild(i).position.z;
            if (loc < curLow)
            {
                curLow = loc;
                val =  i + 1;
            }
        }
        return val;
    }

    void Jump()
    {
        Vector3 rngVector = new Vector3(Random.value - Random.value, Random.value - Random.value, -Random.value);
        Debug.Log("jumping: " + rngVector);
        GetComponent<Rigidbody>().AddForce(rngVector * 500);
        GetComponent<Rigidbody>().AddTorque(rngVector * 200);
    }
}

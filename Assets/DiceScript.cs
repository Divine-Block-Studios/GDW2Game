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
        StaticHelpers.ThrowAt(transform, transform.position, Vector3.zero, 20, 25);
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

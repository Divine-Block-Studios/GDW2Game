using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class debugScript : MonoBehaviour
{
    [Header("Debug")]
    private Quaternion transformQuaternion;
    public Vector3 transformEuler;
    public bool jump;

    // Update is called once per frame


    void Update()
    {
        transformQuaternion = transform.rotation;
        Debug.Log(transformQuaternion);
        transformEuler = transform.eulerAngles;
        if (jump)
        {
            Debug.Log("jumping: ");
            Jump();
            jump = false;
        }
    }

    void Jump()
    {

        Vector3 rngVector = new Vector3(Random.value - Random.value, Random.value - Random.value, -Random.value);
        Debug.Log("jumping: " + rngVector);
        GetComponent<Rigidbody>().AddForce(rngVector * 500);
        GetComponent<Rigidbody>().AddTorque(rngVector * 200);
    }
}

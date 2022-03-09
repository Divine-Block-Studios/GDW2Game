using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour
{
    public List<GameObject> cb = new List<GameObject>();

    bool climbing;

    void Update()
    {
        if(climbing)
        {
            enable();
        }
    }

    void enable()
    {
        Debug.Log("enable climbable object");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        climbing = true;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        climbing = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        climbing = false;
    }
}

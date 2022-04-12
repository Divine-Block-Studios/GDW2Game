using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Cheese : MonoBehaviour
{
    public int value;
    private int baseValue;
    [SerializeField] private GameObject half;
    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private GameObject shinyEffect;
    [SerializeField] private GameObject stinkyEffect;
    [SerializeField] private float pushScalar;
    [SerializeField] private float vertPush;

    private void Awake()
    {
        baseValue = value;
    }


    public void Cut(Vector2 direction, float force)
    {
        print(force);
        
        force *= pushScalar;
        Vector3 rot = Vector3.zero;
        
        rot.z= Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        GameObject goA = Instantiate(half, transform.position, Quaternion.Euler(transform.eulerAngles -rot), transform.parent);
        Rigidbody goARB = goA.GetComponent<Rigidbody>();
        goARB.AddForce(direction * force);
        goARB.AddForce(goA.transform.up * vertPush);
        
        rot.z -= 180;
        GameObject goB = Instantiate(half, transform.position, Quaternion.Euler(transform.eulerAngles -rot), transform.parent);
        Rigidbody goBRB = goB.GetComponent<Rigidbody>();
        goBRB.AddForce(direction * force);
        goBRB.GetComponent<Rigidbody>().AddForce(goB.transform.up * vertPush);
        if (value > baseValue)
        {
            print("Shiny particles");
            Instantiate(shinyEffect, goA.transform);
            Instantiate(shinyEffect, goB.transform);
        }
        else if (value < baseValue)
        {
            print("Stinky particles");
            Instantiate(stinkyEffect, goA.transform);
            Instantiate(stinkyEffect, goB.transform);
        }

        Destroy(gameObject);
        GameObject temp = Instantiate(destructionEffect, transform.position, quaternion.identity);
        Destroy(temp, 1);
        print("Cut AT: " + direction + " force of " + force);


        //Create the slice cheese objects.

        //Get rotation degrees by arccos(adj/hyp)
        //rotate each side by flipped z across rot (180)
        //Add force to each split, dependent on the normal of the direction
        //Destroy this object.
    }
}

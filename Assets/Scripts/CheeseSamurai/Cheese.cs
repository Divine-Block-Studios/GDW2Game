using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Cheese : MonoBehaviour
{
    public int value;
    public LayerMask layer;

    [SerializeField] private GameObject half;
    public void Cut(Vector2 direction)
    {
        gameObject.layer = layer;
        //Create the slice cheese objects.
        
        //Get rotation degrees by arccos(adj/hyp)
        //rotate each side by flipped z across rot (180)
        //Add force to each split, dependent on the normal of the direction
        //Destroy this object.
    }
}

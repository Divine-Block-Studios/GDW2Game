using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cheese : MonoBehaviour
{
    public int value;
    public LayerMask layer;

    [SerializeField] private GameObject half;
    [SerializeField] private float shinyChance;
    [SerializeField] private float stinkyChance;
    [SerializeField] private float basePoints;
    [SerializeField] private float shinyMultiplier;
    [SerializeField] private float stinkyMultiplier;

    private void Awake()
    {
        float rng = Random.Range(0f, 1f);


    }

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

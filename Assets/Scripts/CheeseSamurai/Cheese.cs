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
    [SerializeField] private GameObject shinyParticles;
    [SerializeField] private GameObject stinkyParticles;
    [Range(0,100)]
    [SerializeField] private float shinyChance;
    [Range(0,100)]
    [SerializeField] private float stinkyChance;
    [SerializeField] private int shinyMultiplier;
    [SerializeField] private int stinkyMultiplier;

    private void Awake()
    {
        shinyChance /= 1000;
        stinkyChance /= 1000;

        float rng = Random.Range(0f, 1f);

        //Is stinky
        if (rng < stinkyChance)
        {
            value *= stinkyMultiplier;
            Instantiate(stinkyParticles, transform);
        }
        //Is Shiny
        else if (rng < shinyChance + stinkyChance)
        {
            value *= shinyMultiplier;
            Instantiate(shinyParticles, transform);
        }
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

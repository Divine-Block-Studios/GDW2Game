using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings settings { get; set; }

    public string Name => "Billy Bob";

    private void Awake()
    {
        if (settings != null && settings != this)
        {
            Destroy(gameObject);
        }
        else
        {
            settings = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    [SerializeField]
    private UnityEvent click = new UnityEvent();
    public UnityEvent ONClick { get { return click; } set { click = value; } }


    public void Pressed()
    {
        ONClick.Invoke();
    }
}

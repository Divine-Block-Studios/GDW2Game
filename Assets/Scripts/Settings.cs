using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings settings { get; set; }

    [SerializeField] private TMP_InputField userName;
    public string Name => userName.text;

    public bool HideCodeOnStart => false;

    private void Awake()
    {
        if (userName == null)
        {
            //Literally does nothing, but an error gets thrown without it...
            userName = GameObject.Find("UserName").GetComponent<TMP_InputField>();
        }

        userName.characterLimit = 12;
        userName.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        userName.onValueChanged.AddListener((text) => userName.text = text.ToUpper());
        
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
}

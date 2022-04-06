using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Settings : MonoBehaviour
{
    public static Settings settings { get; set; }

    [SerializeField] private TMP_InputField userName;
    public string Name
    {
        get
        {
            if (userName.text == "")
                userName.text = RandomName();
            return userName.text; 
        }
    }

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

    private string RandomName()
    {
        string[] names1 = {"Silly", "Large", "Quiet", "Upset", "Tiny", "Honked", "Zesty", "Hot", "Cold", "Mega", "Lame", "Light", "Hyper", "Jelly", "Happy", "Insane", "Psycho", "Genius", "Shy", "Tired"};
        string[] names2 = {"Duck", "Cutie", "Goose", "Bagel", "Jinx", "Carlos", "Hotdog", "Vegan", "Shorty", "Probe", "Bone", "Rod", "Hole", "Mole", "Child", "Adult", "Cheese", "Sushi", "Ninja"};
        return names1[Random.Range(0, names1.Length)] + names2[Random.Range(0, names2.Length)];
    }
}

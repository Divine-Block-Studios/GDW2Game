using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Settings : MonoBehaviour
{
    //public static Settings settings { get; set; }
    public TMP_InputField _nameText;
    public TMP_InputField _codeText;
    
    public string Name
    {
        get
        {
            if (_nameText.text == "")
                _nameText.text = RandomName();
            return _nameText.text; 
        }
    }

    public bool HideCodeOnStart => false;

    private void Start()
    {
        _nameText.characterLimit = 12;
        _codeText.characterLimit = 6;
        _nameText.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        _codeText.characterValidation = TMP_InputField.CharacterValidation.Digit;
        _nameText.onValueChanged.AddListener((text) => _nameText.text = text.ToUpper());
    }

    private string RandomName()
    {
        string[] names1 = {"Silly", "Large", "Quiet", "Upset", "Tiny", "Honked", "Zesty", "Hot", "Cold", "Mega", "Lame", "Light", "Hyper", "Jelly", "Happy", "Insane", "Psycho", "Genius", "Shy", "Tired", "Sussy", "Mommy", "Pink", "Swag"};
        string[] names2 = {"Duck", "Cutie", "Goose", "Bagel", "Jinx", "Carlos", "Hotdog", "Vegan", "Shorty", "Probe", "Bone", "Rod", "Hole", "Mole", "Child", "Adult", "Cheese", "Sushi", "Ninja", "Bakka", "Dommy", "Bird", "Possum"};
        return names1[Random.Range(0, names1.Length)] + names2[Random.Range(0, names2.Length)];
    }
}

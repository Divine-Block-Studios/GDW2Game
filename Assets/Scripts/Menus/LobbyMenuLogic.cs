using System;
using System.Collections;
using System.Collections.Generic;
using Menus;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LobbyMenuLogic : MonoBehaviour
{
    [SerializeField] private RecyclablesManagerLobby networkManager;
    [SerializeField] private TMP_InputField roomCode;
    private UnityAction<String> fillRemainder;

    private void Awake()
    {
        roomCode.characterLimit = 6;
        roomCode.characterValidation = TMP_InputField.CharacterValidation.Integer;
        roomCode.onDeselect.AddListener(Fill);
    }

    private void Fill(string text)
    {
        if (text.Length == 0)
            roomCode.text =  "";
        text += "000000";
        roomCode.text =  text.Substring(0, 6);
    }

    public void HostLobby()
    {
        if (roomCode.text == "000000")
        {
            //First Query if this list exists
            networkManager.networkAddress = "10.0.0.182";
        }
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        if (roomCode.text == "000000")
        {
            //First Query if this list exists
            networkManager.networkAddress = "10.0.0.182";
        }
        //Connect on local host.
        networkManager.StartClient();
    }
}

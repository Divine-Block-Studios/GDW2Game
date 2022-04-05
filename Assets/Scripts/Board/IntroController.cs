using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroController : MonoBehaviourPun
{
    [SerializeField]private GameObject postTourCutScene;
    [SerializeField]private TextMeshProUGUI readyText;
    
    private Controls _controls;

    private bool isLocalReady;
    private int readyCount;

    
    public void CreateCutScene()
    {
        if (Application.isPlaying)
            Instantiate(postTourCutScene);
    }
    
    

    private void Start()
    {
        StaticHelpers.Curtains(() => readyText.gameObject.SetActive(true));
        _controls.PCBoardControls.Interact.started += ToggleReady;
        _controls.TouchBoardControls.Interact.started += ToggleReady;
    }

    private void ToggleReady(InputAction.CallbackContext ctx)
    {
        isLocalReady = !isLocalReady;

        int x = isLocalReady ? 1 : -1;
        photonView.RPC("UpdateTextObject", RpcTarget.AllBuffered, x);
    }

    [PunRPC]
    private void UpdateTextObject(int readyAdd)
    {
        readyCount += readyAdd;

        if (readyCount > PhotonNetwork.CurrentRoom.PlayerCount / 2)
        {
            print("Vote passed, Skipping cutscene");
            Finished();
            return;
        }
        readyText.color = Color.white;
        
        readyText.text = isLocalReady ? "Press anywhere to unskip: " : "Press anywhere to skip: ";
        readyText.text += readyCount+"/"+PhotonNetwork.CurrentRoom.PlayerCount;
        StaticHelpers.Fade(readyText, Color.clear, 3, 3);
    }

    public void Finished()
    {
        readyText.gameObject.SetActive(false);
        Destroy(gameObject);
    }


}

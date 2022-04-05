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
    [SerializeField] private TextMeshProUGUI numText;
    
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
        BoardInputControls._controls.PCBoardControls.Interact.started += ToggleReady;
        BoardInputControls._controls.TouchBoardControls.Interact.started += ToggleReady;
        StaticHelpers.Curtains(() => {
            readyText.transform.parent.gameObject.SetActive(true);
            photonView.RPC("UpdateTextObject", RpcTarget.AllBuffered, 0);
        });
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
        numText.color = Color.white;

        readyText.text = isLocalReady ? "PRESS ANYWHERE TO UNSKIP: ": "PRESS ANYWEHRE TO SKIP: ";
        numText.text = readyCount+"/"+PhotonNetwork.CurrentRoom.PlayerCount;
        StaticHelpers.Fade(readyText, Color.clear, 3, 3);
        StaticHelpers.Fade(numText, Color.clear, 3, 3);
    }

    public void Finished()
    {
        //Close, Open remove.
        StaticHelpers.Curtains(() => StaticHelpers.Curtains(()=>
        {
            print("Done Curtains.");
            readyText.transform.parent.gameObject.SetActive(false);
            BoardInputControls._controls.PCBoardControls.Interact.started -= ToggleReady;
            BoardInputControls._controls.TouchBoardControls.Interact.started -= ToggleReady;
            PhotonNetwork.Destroy(gameObject);
        }));
    }


}

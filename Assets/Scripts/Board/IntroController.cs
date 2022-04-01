using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class IntroController : MonoBehaviourPun
{
    [SerializeField]private GameObject postTourCutScene;
    public void CreateCutScene(){
        if(Application.isPlaying)
            Instantiate(postTourCutScene);
    }

    public void ActivateGameManager()
    {
        GameManager.gameManager.
    }
}

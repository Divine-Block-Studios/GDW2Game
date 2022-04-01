using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class IntroController : MonoBehaviourPun
{
    private enum EIntroState
    {
        ShowMap,
        ShowingHost,
        ShowingRecycling,
        RollingDice,
        Completed
    }

    private EIntroState _eIntroState;
    private CinemachineSmoothPath path;
    private Vector3 end;
    private GameObject RecycleUICutscene;

    // Start is called before the first frame update
    void Start()
    {
        _eIntroState = EIntroState.ShowMap;
        DontDestroyOnLoad(gameObject);
        path = GetComponent<CinemachineSmoothPath>();
        end = path.m_Waypoints[path.m_Waypoints.Length-1].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == end && _eIntroState == EIntroState.ShowMap)
        {
            _eIntroState++;
        }
        CheckState();
    }

    private void CheckState()
    {
        switch (_eIntroState)
        {
            case EIntroState.ShowMap:
                return;
            case EIntroState.ShowingHost:
                Wait(15000);
                break;
            case EIntroState.ShowingRecycling:
                Wait(20000);
                PhotonNetwork.Instantiate("Prefabs/Map Assets/"+RecycleUICutscene.name,Vector3.zero, Quaternion.identity);
                break;
            case EIntroState.RollingDice:
                //When Spinner done?
                break;
            case EIntroState.Completed:
                Destroy(gameObject);
                break;
        }
    }

    private async void Wait(int millis)
    {
        await Task.Delay(millis);
        _eIntroState++;
    }
}

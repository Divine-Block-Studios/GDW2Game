using System.Threading.Tasks;
using Board;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class IntroController : MonoBehaviourPun
{
    [SerializeField] private GameObject postTourCutScene;
    [SerializeField] private GameObject HUD;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private TextMeshProUGUI numText;
    
    private Controls _controls;

    private bool isLocalReady;
    private int readyCount;

    private GameObject cutScene;

    private bool skip;

    
    public void CreateCutScene()
    {
        if (Application.isPlaying && !skip)
            cutScene = Instantiate(postTourCutScene);
    }
    
    
    public void Init()
    {
        _controls = GameObject.Find(PhotonNetwork.LocalPlayer.NickName).GetComponent<BoardInputControls>()._controls;
        
        _controls.PCBoardControls.Interact.started += ToggleReady;
        _controls.TouchBoardControls.Interact.started += ToggleReady;
        readyText.transform.parent.gameObject.SetActive(true);
        photonView.RPC("UpdateTextObject", RpcTarget.AllBuffered, 0);
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
            Skip();
            return;
        }
        readyText.color = Color.white;
        numText.color = Color.white;

        readyText.text = isLocalReady ? "PRESS ANYWHERE TO UNSKIP: ": "PRESS ANYWEHRE TO SKIP: ";
        numText.text = readyCount+"/"+PhotonNetwork.CurrentRoom.PlayerCount;
        StaticHelpers.Fade(readyText, Color.clear, 3, 3);
        StaticHelpers.Fade(numText, Color.clear, 3, 3);
    }

    public void Skip()
    {
        //Close, Open remove.
        StaticHelpers.Curtains(() =>
        {
            print("Done Curtains.");
            readyText.transform.parent.gameObject.SetActive(false);
            _controls.PCBoardControls.Interact.started -= ToggleReady;
            _controls.TouchBoardControls.Interact.started -= ToggleReady;
            if (cutScene)
                Destroy(cutScene);
            skip = true;
            transform.GetComponent<PlayableDirector>().time = 107.75f;
            AudioSource[] arr = transform.GetComponents<AudioSource>();
            foreach(AudioSource audioSource in arr)
            {
                print("Removing");
                StaticHelpers.MuteAudio(audioSource, 0, 0.4f);
            }
            StaticHelpers.Curtains(null);
        });
    }

    public void BeginGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Item[] playersAsAwards = GameManager.gameManager.playersAsItems;

            GameManager.gameManager.CreateSelectionUI(playersAsAwards, true, false, null, 1, async () =>
            {
                print("Done Spinning.");
                photonView.RPC("SetPlayers", RpcTarget.AllBuffered);
                await Task.Delay(1000);
                photonView.RPC("ChangeToPlayerSpec", RpcTarget.AllBuffered);
            });
        }
    }

    [PunRPC]
    private void SetPlayers()
    {
        float rot = 360 / GameManager.gameManager.players.Length;
        float dist = 3;
        for (int x = 0; x < GameManager.gameManager.players.Length; x++)
        {
            Vector3 temp = Vector3.zero;
            
            temp.x = Mathf.Cos(rot * x * Mathf.Deg2Rad) -
                     Mathf.Sin(rot * x * Mathf.Deg2Rad);
            temp.z = Mathf.Sin(rot * x * Mathf.Deg2Rad) +
                     Mathf.Cos(rot * x * Mathf.Deg2Rad);
            print("Gamma: " + temp + " --> " + GameManager.gameManager.players[x]);
            GameManager.gameManager.players[x].offSet += temp * dist;
        }
        GameManager.gameManager.MyPlayer.Teleport(GameManager.gameManager.MyPlayer.currentTile.transform.position, true);
    }

    [PunRPC]
    private void ChangeToPlayerSpec()
    {
        _controls.PCBoardControls.Interact.started -= ToggleReady;
        _controls.TouchBoardControls.Interact.started -= ToggleReady;
        HUD.SetActive(true);
        GameManager.gameManager.BeginGame();
        Destroy(gameObject);
    }
    
    
    

    private void OnDestroy()
    {
        Transform t = Camera.main.transform;
        t.rotation = Quaternion.identity;
        t.localPosition = new Vector3(0, 0, -20f);
    }
}

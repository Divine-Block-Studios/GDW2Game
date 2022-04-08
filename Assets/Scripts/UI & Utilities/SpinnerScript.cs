using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class SpinnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float minSpinTimeS;
    [SerializeField] private float maxSpinTimeS;
    [SerializeField] private int delayMS;
    [SerializeField] private int postDestructionDelay;
    [SerializeField] private float radius;
    [SerializeField] private Material matA;
    [SerializeField] private Material matB;
    [SerializeField] private Material spriteMaterial;

    private float _trueSpinTime;
    
    private float _curForce;
    private int _curTile;

    private List<MeshRenderer> _cones;

    private Action post;

    public PhotonView _photonView;

    private AwardableEvents[] items;

    private float angle; // Need angle for update.
    private int _count;

    private float prvRot;
    private float nxtRot = 0;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }


    public void Init(AwardableEvents[] items, BoardPlayer ply = null, Action onComplete = null)
    {
        print("initing");
        post = onComplete;
        _trueSpinTime = Random.Range(minSpinTimeS, maxSpinTimeS);
        this.items = items;

        //1) Photonnetwork Create a temp object
        //2) Set the image and name of the temp object to be the correct object
        //3) Everyone just has to reference the same object. w/ Gameobject.Find();

        string[] temp = new string[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] is Item)
            {
                temp[i] = "Items/"+items[i].name;
            }
            else
            {
                temp[i] = "MiniGames/"+items[i].name;
            }

            
            print(temp[i]);
        }
        _photonView.RPC("DrawConeRPC", RpcTarget.AllBuffered, temp as object);
        
        Spin(items, ply);
    }
    
    public void Init(BoardPlayer ply = null, Action onComplete = null)
    {
        print("initing");
        post = onComplete;
        _trueSpinTime = Random.Range(minSpinTimeS, maxSpinTimeS);

        string[] temp = {};
        _photonView.RPC("DrawConeRPC", RpcTarget.AllBuffered, temp as object);
        items = GameManager.gameManager.playersAsItems;
        Spin(items, ply);
    }


    //This may be slow drawing the shape again and again...
    [PunRPC]
    private void DrawConeRPC(object [] assetNames)
    {
        AwardableEvents [] awardableEvents = new AwardableEvents[assetNames.Length];
        if (assetNames.Length > 0)
        {
            for (int i = 0; i < assetNames.Length; i++)
            {
                awardableEvents[i] = Resources.Load<AwardableEvents>("LoadableAssets/" + assetNames[i]);
            }
        }
        else
        {
            Debug.Log("Successfully got assets");
            awardableEvents = GameManager.gameManager.playersAsItems;
        }
        
        _cones = new List<MeshRenderer>();
        _count = awardableEvents.Length;
        angle = 360f / _count;
        
        //140 is magic spinner number, must always start at 140
        prvRot = 140;
        transform.parent.eulerAngles = new Vector3(-90, 0, 0);
        transform.localEulerAngles = new Vector3(0, 0, prvRot);

        for (int index = 0; index < awardableEvents.Length; index++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            //items may be null. I need to send a sprite??? and a string for names
            go.name = "SpinnerSlice: " + index + " ( " + awardableEvents[index].awardName + " )";
            Mesh mesh = new Mesh();
            go.AddComponent<MeshFilter>().mesh = mesh;
            MeshRenderer re = go.AddComponent<MeshRenderer>();

            GameObject item = new GameObject();

            item.transform.SetParent(go.transform);
            item.name = awardableEvents[index].awardName + ": Image";
            SpriteRenderer sr = item.AddComponent<SpriteRenderer>();
            sr.material = spriteMaterial;

            sr.sprite = awardableEvents[index].icon;

            re.receiveShadows = false;
            re.shadowCastingMode = ShadowCastingMode.Off;
            re.material = matA;

            float rotation = (angle * (index + 1)) % 360;
            int rayCount = (_count > 12) ? 3 : 32 / _count;
            //unsure
            float angIncrease = angle / rayCount;

            Vector3[] vertices = new Vector3[rayCount + 2]; //For first and last
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            int vertexIndex = 1;
            int trianglesIndex = 0;
            float tempRot = rotation - angle / 2;
            item.transform.localEulerAngles = new Vector3(180, 0,-tempRot);
            item.transform.localPosition =
                new Vector3(Mathf.Sin(tempRot * Mathf.Deg2Rad), Mathf.Cos(tempRot * Mathf.Deg2Rad)) * (radius / 1.5f) +
                new Vector3(0, 0, 1f);

            //item.transform.localPosition = new Vector3()


            for (int i = 0; i <= rayCount; i++)
            {
                //Get's the vertices point rotated

                Vector3 vertex = new Vector3(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad)) *
                                 radius;
                vertices[vertexIndex] = vertex;

                if (i > 0)
                {
                    //Draw a triangle
                    triangles[trianglesIndex] = 0; // left most
                    triangles[trianglesIndex + 1] = vertexIndex - 1; // Middle
                    triangles[trianglesIndex + 2] = vertexIndex; // right most

                    trianglesIndex += 3;
                }

                vertexIndex++;
                rotation -= angIncrease;
            }

            //Don't know why this is appropriate, but it is... So whatever..
            float size = Mathf.Min(radius * 0.66f, Vector3.Distance(vertices[0], vertices[rayCount/2]));
            item.transform.localScale = new Vector3(size, size, 1);
            go.transform.localPosition = Vector3.zero;
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            _cones.Add(re);
        }
        _cones[_curTile].material = matB;
    }

    private void Update()
    {
        if (transform.localEulerAngles.z < prvRot && transform.localEulerAngles.z > nxtRot)
        {
            _cones[_curTile].material = matA;
            if (--_curTile < 0)
            {
                _curTile = _cones.Count -1; 
            }
            _cones[_curTile].material = matB;

            prvRot -= angle;
            if (prvRot < 0)
            {
                prvRot = 360 + prvRot;
                nxtRot = prvRot - angle;
            }
            else
            {
                nxtRot = 0;
            }
        }
    }

    private async void Spin(AwardableEvents[]items, BoardPlayer ply)
    {
        print("Beginning spin");
        
        _curForce = Random.Range(minForce, maxForce);
        float time = 0;
        float initForce = _curForce;
        await Task.Delay(delayMS);

        while (initForce > 0)
        {
            initForce = _curForce - _curForce * (time / _trueSpinTime);
            transform.localEulerAngles -= new Vector3(0, 0, initForce);
            time += Time.deltaTime;
            await Task.Delay(1);
        }
        //Acts as a callback.
        print("Spinner landed on: " + items[_curTile].name) ;
        await Task.Delay(postDestructionDelay);
        if (ply)
        {
            items[_curTile].Init(ply);
        }
        else
        {
            _photonView.RPC("PlayerStartFrom", RpcTarget.AllBuffered, _curTile);
        }
        post?.Invoke();
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void PlayerStartFrom(int index)
    {
        StaticHelpers.StartFrom(ref GameManager.gameManager.players, index);
    }
}

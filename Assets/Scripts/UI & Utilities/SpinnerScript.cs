using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
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
    [SerializeField] private Material tempA;
    [SerializeField] private Material tempB;

    private float _trueSpinTime;
    
    private float _curForce;
    private int _count;
    private int _curTile;

    private List<MeshRenderer> _cones;

    private Material matA;
    private Material matB;

    public void Init(AwardableEvents [] items, BoardPlayer ply)
    {
        matA = tempA;
        matB = tempB;
        _cones = new List<MeshRenderer>();
        _count = items.Length;
        _trueSpinTime = Random.Range(minSpinTimeS, maxSpinTimeS);
        
        float angle = 360f / _count;
        
        for (int i = 0; i < _count; i++)
        {
            DrawCone(angle, i, items);
        }
        Spin(angle, items, ply);
    }

    //This may be slow drawing the shape again and again...
    private void DrawCone(float angle, int index, AwardableEvents [] items)
    {
        
        GameObject go = Instantiate(new GameObject() ,transform);
        go.name = "SpinnerSlice: " + index + " ( " + items[index].name + " )";
        Mesh mesh = new Mesh();
        go.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer re = go.AddComponent<MeshRenderer>();
        
        GameObject item = Instantiate(new GameObject(), go.transform);
        item.name = items[index].name + ": Image";
        SpriteRenderer sr = item.AddComponent<SpriteRenderer>();

        //Weird magic float I guess
        
        //float size = Mathf.Min(1/ , maxHeight);
        
        
        sr.sprite = items[index].icon;

        re.receiveShadows = false;
        re.shadowCastingMode = ShadowCastingMode.Off;
        re.material = matA;

        float rotation = (angle * (index + 1)) % 360;
        int rayCount = (_count > 12)?3:32/_count;
        print(rayCount);
        //unsure
        float angIncrease = angle / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2]; //For first and last
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        int vertexIndex = 1;
        int trianglesIndex = 0;
        float tempRot = rotation - angle / 2;
        item.transform.eulerAngles = new Vector3(0,0,-tempRot);
        item.transform.localPosition = new Vector3(Mathf.Sin(tempRot * Mathf.Deg2Rad), Mathf.Cos(tempRot * Mathf.Deg2Rad)) * (radius / 1.5f) + new Vector3(0,0,-1f);
        
        //item.transform.localPosition = new Vector3()
        
        
        for (int i = 0; i <= rayCount; i++)
        {
            //Get's the vertices point rotated
            
            Vector3 vertex = new Vector3(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad)) * radius;
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
        float maxHeight = radius * 0.025f;
        float size = Mathf.Min(maxHeight, Vector3.Distance(vertices[0], item.transform.localPosition) / 8 / (_count/2));
        item.transform.localScale = new Vector3( size,size, 1);
        
        print(Vector3.Distance(vertices[0], item.transform.localPosition));

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        _cones.Add(re);
        print(Vector3.Distance(item.transform.position, transform.position));
    }

    private async void Spin(float angle, AwardableEvents[]items, BoardPlayer ply)
    {
        _curForce = Random.Range(minForce, maxForce);
        float time = 0;
        float initForce = _curForce;
        _cones[_curTile].material = matB;
        await Task.Delay(delayMS);


        while (initForce > 0)
        {
            initForce = _curForce - _curForce * (time / _trueSpinTime);
            transform.eulerAngles -= new Vector3(0, 0, initForce);
            
            time += Time.deltaTime;
            
            if (transform.localEulerAngles.z < angle * _curTile || (_curTile == 0 && transform.localEulerAngles.z > 358))
            {
                //Change skins
                _cones[_curTile].material = matA;
                if (--_curTile < 0)
                {
                    _curTile = _cones.Count -1; 
                }
                _cones[_curTile].material = matB;
                //Check if curTile is too high, reset if so.
            }
            await Task.Delay(1);
        }
        //Acts as a callback.
        await Task.Delay(postDestructionDelay);
        items[_curTile].Init(ply);
        Destroy(gameObject);
        
    }
}

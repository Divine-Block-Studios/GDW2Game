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

    public void Init(AwardableEvents [] items, Vector3 loc, BoardPlayer ply)
    {
        matA = tempA;
        matB = tempB;
        _cones = new List<MeshRenderer>();
        _count = items.Length;
        _trueSpinTime = Random.Range(minSpinTimeS, maxSpinTimeS);
        
        float angle = 360f / _count;
        
        print("TEST: Beginning Draw Cone");
        for (int i = 0; i < _count; i++)
        {
            DrawCone(angle, i, loc);
        }
        Spin(angle, items, ply);
    }

    //This may be slow drawing the shape again and again...
    private void DrawCone(float angle, int index, Vector3 location)
    {
        GameObject go = Instantiate(new GameObject() ,transform);
        Mesh mesh = new Mesh();
        go.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer re = go.AddComponent<MeshRenderer>();
        print("index: " +index);
        
        re.receiveShadows = false;
        re.shadowCastingMode = ShadowCastingMode.Off;
        re.material = matA;

        float rotation = (angle * (index + 1)) % 360;
        print(rotation);
        int rayCount = 24/_count;
        //unsure
        float angIncrease = angle / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2]; //For first and last
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = location;

        int vertexIndex = 1;
        int trianglesIndex = 0;
        
        for (int i = 0; i <= rayCount; i++)
        {
            //Get's the vertices point rotated
            
            Vector3 vertex = location + new Vector3(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad)) * radius;
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

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        print("TEST: Cone Complete");
        _cones.Add(re);
    }

    private async Task Spin(float angle, AwardableEvents[]items, BoardPlayer ply)
    {
        _curForce = Random.Range(minForce, maxForce);
        print("TEST: Beginning Spin");
        float time = 0;
        float initForce = _curForce;
        float angForNext = angle;
        _cones[_curTile].material = matB;
        await Task.Delay(delayMS);


        while (initForce > 0)
        {
            //print("Force: " + initForce + " - " + time / spinTimeS);
            initForce = _curForce - _curForce * (time / _trueSpinTime);
            transform.eulerAngles -= new Vector3(0, 0, initForce);
            
            time += Time.deltaTime;
            
            if (((int)transform.localEulerAngles.z / (int)angForNext) != _curTile)
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
        print("TEST: Finished: " + items[_curTile]);
        items[_curTile].Init(ply);
        //Activate Item.
        Destroy(gameObject);
        
    }
}

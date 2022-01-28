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
    [SerializeField] private float spinTime;
    [SerializeField] private int delay;
    [SerializeField] private float radius;
    [SerializeField] private Material tempA;
    [SerializeField] private Material tempB;
    
    private float _curForce;
    private int _count;

    private List<MeshRenderer> _cones;

    private Material matA;
    private Material matB;

    private void Start()
    {
        List<string> testing = new List<string> {"A", "B", "C", "D", "A", "B"};
        Init(testing, new Vector3(0,0,15), tempA, tempB);
    }

    public void Init<T>(List<T> items, Vector3 loc, Material A, Material B)
    {
        matA = A;
        matB = B;
        _cones = new List<MeshRenderer>();
        _count = items.Count;
        
        float angle = 360f / _count;
        print("Range: " + angle);
        
        for (int i = 0; i < _count; i++)
        {
            DrawCone(angle, i, loc);
        }
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
        int rayCount = 18/_count;
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
            print("Vertex: " + vertex);
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

        print("Drawn Cone");
        _cones.Add(re);
        StartCoroutine(Spin(angle));
    }

    private IEnumerator Spin(float angle)
    {
        _curForce = Random.Range(minForce, maxForce);
        print(_curForce);
        float time = 0;
        float initForce = _curForce;
        float angForNext = angle;
        int curTile = 0;
        _cones[curTile].material = matB;
        yield return new WaitForSeconds(delay);


        while (initForce > 0)
        {
            print("Force: " + initForce + " - " + time / spinTime);
            initForce = _curForce - _curForce * (time / spinTime);
            transform.eulerAngles -= new Vector3(0, 0, initForce);
            
            time += Time.deltaTime;
            
            if (((int)transform.eulerAngles.z / (int)angForNext) != curTile)
            {
                //Change skins
                _cones[curTile].material = matA;
                if (--curTile < 0)
                {
                    curTile = _cones.Count -1; 
                }
                _cones[curTile].material = matB;
                //Check if curTile is too high, reset if so.
                
            }

            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }
}

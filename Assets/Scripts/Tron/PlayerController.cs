using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public List<Rigidbody2D> rb;
    public List<GameObject> trails = new List<GameObject>();
    public List<TrailRenderer> tr;

    public Camera cam;

    public float speed;
    string lastPressed;

    void Awake()
    {
        for (int i = 0; i < players.Count; i++)
        {
            rb.Add(players[i].GetComponent<Rigidbody2D>());
            rb[i].isKinematic = false;
            tr.Add(players[i].GetComponent<TrailRenderer>());
            trails[i].GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }

    void Update()
    {
        movement();
        createMesh();
    }

    void movement()
    {
        if (Input.GetKeyDown(KeyCode.W) && lastPressed != "S")
        {
            rb[0].velocity = new Vector2(0f, 1f * speed);
            lastPressed = "W";
        }
        if (Input.GetKeyDown(KeyCode.S) && lastPressed != "W")
        {
            rb[0].velocity = new Vector2(0f, -1f * speed);
            lastPressed = "S";
        }
        if (Input.GetKeyDown(KeyCode.D) && lastPressed != "A")
        {
            rb[0].velocity = new Vector2(1f * speed, 0f);
            lastPressed = "D";
        }
        if (Input.GetKeyDown(KeyCode.A) && lastPressed != "D")
        {
            rb[0].velocity = new Vector2(-1f * speed, 0f);
            lastPressed = "A";
        }
    }

    void createMesh()
    {
        if (tr != null)
        {
            Mesh mesh = new Mesh();
            tr[0].BakeMesh(mesh, cam, true);
            trails[0].GetComponent<MeshFilter>().mesh = mesh;

            PolygonCollider2D collider = trails[0].GetComponent<PolygonCollider2D>();

            List<Vector2> verts = new List<Vector2>();

            foreach (Vector2 vertex in mesh.vertices)
            {
                verts.Add(vertex);
            }
            collider.points = verts.ToArray();
        }
    }
}

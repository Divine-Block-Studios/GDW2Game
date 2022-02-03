using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    public List<Rigidbody2D> rb;
    public List<GameObject> trails = new List<GameObject>();
    public MeshFilter mesh;

    private List<Vector2> points;
    private Vector3 offset;
    private Vector2 boxSize;
    private Vector2 cornerPoint;
    [SerializeField] private EdgeCollider2D EC;


    public Camera cam;

    public float speed;
    string lastPressed;

    private bool _canTurn = true;

    void Awake()
    {
        //curColl = colliderTransform.GetComponent<BoxCollider2D>();
        points = new List<Vector2>();
        //Assume Player will move up.
        boxSize = players[0].GetComponent<RectTransform>().sizeDelta;
        boxSize /= 2;
        offset = Vector2.down * boxSize.y;
        EC.enabled = false;
        points.Add(players[0].transform.position + offset);
        //EC = players[0].transform.GetChild(0).GetComponent<EdgeCollider2D>();

        for (int i = 0; i < players.Count; i++)
        {
            rb.Add(players[i].GetComponent<Rigidbody2D>());
            rb[i].isKinematic = false;
            //tr.Add(players[i].GetComponent<TrailRenderer>());
            //trails[i].GetComponent<Rigidbody2D>().isKinematic = false;
        }

    }

    void Update()
    {
        if (EC.enabled)
        {
            print("Running");
            points[points.Count - 1] = players[0].transform.position + offset;
            EC.SetPoints(points);
            print(cornerPoint + " - " + offset + " - " + players[0].transform.position);
            if (!_canTurn)
            {
                _canTurn = CheckIfCanTurn();
                if (_canTurn)
                {
                    if (points.Count > 2)
                        points[points.Count - 2] = cornerPoint;
                    print("Placing: " + (cornerPoint - (Vector2)offset * 2) + " - " + players[0].transform.position);
                }
            }

            else
            {
                print("Turning Enabled");
                movement();
            }
        }
        else
        {
            movement();
        }
    }

    void movement()
    {
        if (Input.GetKeyDown(KeyCode.W) && lastPressed != "S")
        {

            lastPressed = "W";
            points.Add(players[0].transform.position + offset);
            cornerPoint = players[0].transform.position;


            offset = Vector2.down * boxSize.y;
            //points[points.Count - 3] = players[0].transform.position + offset;
            EC.enabled = true;
            _canTurn = false;

        }
        else if (Input.GetKeyDown(KeyCode.S) && lastPressed != "W")
        {
            lastPressed = "S";
            points.Add(players[0].transform.position + offset);
            cornerPoint = players[0].transform.position;
            offset = Vector2.up * boxSize.y;
            //points[points.Count - 3] = players[0].transform.position + offset;
            EC.enabled = true;
            _canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.D) && lastPressed != "A")
        {
            lastPressed = "D";
            points.Add(players[0].transform.position + offset);
            cornerPoint = players[0].transform.position;
            offset = Vector2.left * boxSize.x;
            //points[points.Count - 3] = players[0].transform.position + offset;
            EC.enabled = true;
            _canTurn = false;
        }
        else if (Input.GetKeyDown(KeyCode.A) && lastPressed != "D")
        {
            lastPressed = "A";
            points.Add(players[0].transform.position + offset);
            cornerPoint = players[0].transform.position;
            offset = Vector2.right * boxSize.x;
            //points[points.Count - 3] = players[0].transform.position + offset;
            EC.enabled = true;
            _canTurn = false;
        }

        switch (lastPressed)
        {
            case "W":
                rb[0].velocity = new Vector2(0f, 1f * speed);
                break;
            case "S":
                rb[0].velocity = new Vector2(0f, -1f * speed);
                break;
            case "D":
                rb[0].velocity = new Vector2(1f * speed, 0f);
                break;
            case "A":
                rb[0].velocity = new Vector2(-1f * speed, 0f);
                break;
        }
    }

    bool CheckIfCanTurn()
    {
        switch (lastPressed)
        {
            case "W":
                if (cornerPoint.y < (players[0].transform.position.y + offset.y))
                {
                    if (points.Count <= 2)
                        points[0] = cornerPoint;
                    return true;
                }
                break;
            case "S":
                if (cornerPoint.y > (players[0].transform.position.y + offset.y))
                {
                    return true;
                }
                break;
            case "D":
                if (cornerPoint.x < (players[0].transform.position.x + offset.x))
                {
                    return true;
                }
                break;
            case "A":
                if (cornerPoint.x > (players[0].transform.position.x + offset.x))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    void FixCornerPoints()
    {
        if (points.Count > 2)
            points[points.Count - 2] = cornerPoint;
        print("Placing: " + (cornerPoint - (Vector2)offset * 2) + " - " + players[0].transform.position);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kill Player
        // Remove line and colliders
    }

    /*

    void createMesh()
    {
        if (tr != null)
        {
            Mesh mesh = new Mesh();
            tr[0].BakeMesh(mesh, cam);
            trails[0].GetComponent<MeshFilter>().mesh = mesh;

            PolygonCollider2D collider = trails[0].GetComponent<PolygonCollider2D>();

            List<Vector2> verts = new List<Vector2>();
            print("Mesh Verticies: " + mesh.vertices);
            foreach (Vector2 vertex in mesh.vertices)
            {
                verts.Add(vertex);
            }
            collider.points = verts.ToArray();
        }
    }*/
}

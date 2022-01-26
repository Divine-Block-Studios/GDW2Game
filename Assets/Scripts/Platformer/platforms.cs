using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platforms : MonoBehaviour
{
    public List<GameObject> plats = new List<GameObject>();
    public PlayerMovement player;

    Rigidbody2D body;

    List<int> xScale = new List<int>();
    List<int> yScale = new List<int>();

    bool falling;
    int elapsedFrames;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = true;

        for (int i = 0; i < plats.Count; i++)
        {
            plats[i].GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;
        float playerY = playerPos.y - 1f;

        Transform curPlat = plats[0].transform;

        if (elapsedFrames > 30)
        {
            falling = false;
        }

        if (!falling)
        {
            if (playerPos.x > (plats[0].transform.position.x - (curPlat.localScale.x / 2)) && playerPos.x < (plats[0].transform.position.x + (curPlat.localScale.x / 2)) && playerY > plats[0].transform.position.y)
            {
                enablePlatform(0);
            }
            else
            {
                enablePlatform(1);
            }
        }
        elapsedFrames++;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKey(KeyCode.S))
        {
            falling = true;
            elapsedFrames = 0;

            enablePlatform(1);
        }

        //Transform curPlat = plats[int.Parse(collision.transform.tag)].transform;
    }

    public void enablePlatform(int x)
    {
        if (x == 0)
        {
            plats[0].GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else if (x == 1)
        {
            plats[0].GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}

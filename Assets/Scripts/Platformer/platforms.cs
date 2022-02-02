using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platforms : MonoBehaviour
{
    public List<GameObject> plats = new List<GameObject>();
    public PlayerMovement player;

    Rigidbody2D body;

    bool falling;
    int elapsedFrames;
    int currentPlatform;

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
        float playerY = playerPos.y;

        Transform curPlat = plats[currentPlatform].transform;

        if (elapsedFrames > 100)
        {
            falling = false;
        }

        if (!falling)
        {
            if (playerPos.x > (curPlat.position.x - (curPlat.localScale.x / 2)) && playerPos.x < (curPlat.position.x + (curPlat.localScale.x / 2)) && playerY > curPlat.position.y)
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
    }

    public void enablePlatform(int x)
    {
        if (x == 0)
        {
            plats[currentPlatform].GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else if (x == 1)
        {
            plats[currentPlatform].GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    public void recievePlatform(int x)
    {
        currentPlatform = x;
    }
}

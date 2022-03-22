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
    int prevPlat;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

    }

    /*
     
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
                enablePlatform(1, prevPlat);
                enablePlatform(0, currentPlatform);
            }
            else
            {
                enablePlatform(1, currentPlatform);
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

            enablePlatform(1, currentPlatform);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        enablePlatform(1, currentPlatform);
    }

    public void enablePlatform(int x, int platform)
    {
        if (x == 0)
        {
            prevPlat = prevPlatform();
            plats[platform].GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else if (x == 1)
        {
            plats[platform].GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    public void recievePlatform(int x)
    {
        currentPlatform = x;
    }

    int prevPlatform()
    {
        return currentPlatform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enablePlatform(1, currentPlatform);
    }

    */
}
    
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCube : MonoBehaviour
{
    
    [SerializeField] private GameObject powerUp;
    
    private Vector2 boxMin;
    private Vector2 boxMax;

    [Header("Debug")]
    [SerializeField] private bool isUsed;

    // Start is called before the first frame update
    void Awake()
    {
        boxMin = GetComponent<SpriteRenderer>().bounds.min;
        boxMax = GetComponent<SpriteRenderer>().bounds.max;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || isUsed)
            return;
        ContactPoint2D [] collisionPoints = other.contacts;
        for (int i = 0; i < collisionPoints.Length; i++)
        {
            print(collisionPoints[i].point.y + " <= " + boxMin.y);
            if (collisionPoints[i].point.y < boxMin.y)
            {
                isUsed = true;
                print("TRUE");
                GameObject go = Instantiate(powerUp, transform.position + new Vector3(0,boxMax.y, 0), Quaternion.identity);
                print(go.transform.position);
                go.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, other.gameObject.GetComponent<PlayerMovement>()._useJumpPower / 50));
                go.GetComponent<PowerUp>().moveSpeedX *= (other.gameObject.GetComponent<PlayerMovement>().mDir.x < 0)?-1:1;
            }
        }
    }
}

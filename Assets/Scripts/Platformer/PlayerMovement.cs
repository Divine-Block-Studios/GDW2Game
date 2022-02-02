using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public platforms plats;

    Rigidbody2D body;

    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] float fallSpeed;

    float moveDist;
    bool colliding;
    int elapsedFrames;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;

        moveDist = 0.1f * speed;
        elapsedFrames = 200;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            if (colliding)
            {
                body.AddForce(transform.up * jumpPower, ForceMode2D.Force);
            }
        }

        if (Input.GetKey(KeyCode.L) && colliding)
        {
            moveDist = 0.2f * speed;
        }
        else
        {
            moveDist = 0.1f * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 newPos = new Vector3(currentPos.x + moveDist, currentPos.y, currentPos.z);
            transform.position = Vector3.Lerp(currentPos, newPos, 1);

            if (Input.GetKey(KeyCode.L) && !colliding && elapsedFrames >= 100)
            {
                transform.position = new Vector3(currentPos.x + 5f, currentPos.y, currentPos.z);
                elapsedFrames = 0;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 newPos = new Vector3(currentPos.x + -moveDist, currentPos.y, currentPos.z);
            transform.position = Vector3.Lerp(currentPos, newPos, 1);

            if (Input.GetKey(KeyCode.L) && !colliding && elapsedFrames >= 100)
            {
                transform.position = new Vector3(currentPos.x - 5f, currentPos.y, currentPos.z);
                elapsedFrames = 0;
            }
        }

        elapsedFrames++;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        colliding = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        colliding = false;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        int curPlat = int.Parse(collision.tag);

        plats.recievePlatform(curPlat);
    }
}

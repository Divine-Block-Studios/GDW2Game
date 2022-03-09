using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public platforms plats;
    public PlatformerUI ui;
    public PlatformerControls pc;

    Rigidbody2D body;

    [SerializeField] float speed;
    [SerializeField] float jumpPower;

    float moveDist;
    bool colliding;
    bool dashing;
    bool moving;
    public int elapsedFrames;
    Vector2 mDir;

    void Start()
    {
        pc = new PlatformerControls();
        OnEnable();
        InitInputActions();

        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;

        moveDist = 0.1f * speed;
        elapsedFrames = 200;
    }

    void FixedUpdate()
    {
        if (moving || dashing)
        {
            movement();
        }
        elapsedFrames++;
    }

    void OnEnable()
    {
        pc.Enable();
    }
    void OnDisable()
    {
        pc.Disable();
    }

    void InitInputActions()
    {
        pc.PlatformerDefault.Dash.performed += ctx => dashing = true;
        pc.PlatformerDefault.Dash.canceled += ctx => dashing = false;

        pc.PlatformerDefault.Movement.performed += ctx => movementCheck(ctx.ReadValue<Vector2>());
        pc.PlatformerDefault.Movement.canceled += ctx => movementCheck(new Vector2());

        pc.PlatformerDefault.Jump.performed += ctx => jump(ctx.ReadValue<Vector2>());
    }

    void movementCheck(Vector2 moveDir)
    {
        if(moveDir != new Vector2())
        {
            mDir = moveDir;
            moving = true;
        } 
        else
        {
            moving = false;
        }
    }

    void movement()
    {
        Vector2 currentPos = transform.position;

        if(dashing && colliding)
        {
            moveDist = 0.2f * speed;
        }
        else
        {
            moveDist = 0.1f * speed;
        }

        if(moving)
        {
            Vector2 newPos = new Vector2(currentPos.x + moveDist * mDir.x, currentPos.y);
            transform.position = newPos;

            if (dashing && !colliding && elapsedFrames >= 50)
            {
                transform.position = new Vector2(currentPos.x + 5f * mDir.x, currentPos.y);
                elapsedFrames = 0;
                ui.checkDash();
            }
        }
    }

    void jump(Vector2 dir)
    {
        if(colliding)
        {
            body.AddForce(dir * jumpPower, ForceMode2D.Force);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        colliding = true;
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        colliding = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        colliding = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        int curPlat = int.Parse(collision.tag);

        plats.recievePlatform(curPlat);
    }
}

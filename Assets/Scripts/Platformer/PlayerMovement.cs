using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    public platforms plats;
    public PlatformerUI ui;
    public PlatformerControls pc;

    Rigidbody2D body;

    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    
    private float _useSpeed;
    private float _useJumpPower;

    float moveDist;
    bool colliding;
    bool dashing;
    bool moving;
    public int elapsedFrames;
    Vector2 mDir;
    
    private void Awake()
    {
        pc = new PlatformerControls();

    }

    void Start()
    {
        if (isLocalPlayer)
        {
            Debug.Log(isLocalPlayer);
            

            
            InitInputActions();
        }

        _useSpeed = speed;
        _useJumpPower = jumpPower;

        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;

        moveDist = 0.1f * speed;
        elapsedFrames = 200;
    }

    void FixedUpdate()
    {
        if (moving || dashing && isLocalPlayer)
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
            moveDist = 0.2f * _useSpeed;
        }
        else
        {
            moveDist = 0.1f * _useSpeed;
        }

        if(moving)
        {
            Vector2 newPos = new Vector2(currentPos.x + moveDist * mDir.x, currentPos.y);
            Debug.Log(newPos);
            Debug.Log(moveDist);
            Debug.Log(mDir);
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
            body.AddForce(dir * _useJumpPower, ForceMode2D.Force);
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
        Debug.Log("w");
        int curPlat = int.Parse(collision.tag);

        plats.recievePlatform(curPlat);
    }
    
    private void ResetPlayer()
    {
        _useSpeed = speed;
        _useJumpPower = jumpPower;
    }

    public void EndPowerUp()
    {
        Debug.Log("Ending power");
        ResetPlayer();
    }

    public void JumpBoostPowerUp(float increase)
    {
        Debug.Log("Jump power");
        _useJumpPower += increase;
    }
    
}

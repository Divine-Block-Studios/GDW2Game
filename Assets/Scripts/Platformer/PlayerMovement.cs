using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    PlatformerUI ui;
    public PlatformerControls pc;
    public Camera cam;

    Rigidbody2D body;

    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    
    public float _useSpeed;
    public float _useJumpPower;

    float moveDist;
    bool colliding;
    bool dashing;
    bool moving;
    public int elapsedFrames;
    Vector2 mDir;

    bool falling;
    float fallTime;
    
    private void Awake()
    {
        pc = new PlatformerControls();
        cam.enabled = false;

    }

    void Start()
    {
        if (isLocalPlayer)
        {
            cam.enabled = true;
            gameObject.GetComponentInChildren<Canvas>().enabled = true;
            ui = GetComponentInChildren<PlatformerUI>();
            
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

    void Update()
    {
        if(falling && fallTime < 0.2)
        {
            fallTime += Time.deltaTime;
        }
        else
        {
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
            fallTime = 0;
        }
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
        if(isLocalPlayer && collision.gameObject.layer == 9)
        {
            if(Input.GetKey(KeyCode.S))
            {
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                falling = true;
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        colliding = false;
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
    
}

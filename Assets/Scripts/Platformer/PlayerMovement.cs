using System;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public bool _shouldSlam;

    float moveDist;
    bool colliding;
    bool dashing;
    bool moving;
    public int elapsedFrames;
    public Vector2 mDir;

    bool falling;
    float fallTime;

    //Multiplied by gravity
    [SerializeField] private float slamScalar = 5f;
    //Drop off is applied by percentage from dist.
    [SerializeField]  private float slamDist = 5f;
    //Last recorded gravity.
    private float slam;
    
    private void Awake()
    {
        pc = new PlatformerControls();
        cam.enabled = false;

        slamScalar = 1;
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

    private void LateUpdate()
    {
        //NEEDS TO BE CALLED LAST, therefore in late update.
        slam = body.velocity.y;
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

        //Has hit a floor, and can slam
        if (body.velocity.y == 0 && _shouldSlam && slam != 0)
        {
            Slam();
        }
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
        _shouldSlam = false;
    }

    private void Slam()
    {
        RaycastHit2D [] circleCollider2D = new RaycastHit2D[4];
        Physics2D.CircleCast(transform.position, slamDist, Vector2.up, new ContactFilter2D(), circleCollider2D);
        for (int i = 0; i < circleCollider2D.Length; i++)
        {
            if (circleCollider2D[i].transform.gameObject != gameObject)
            {
                if (circleCollider2D[i].rigidbody)
                {
                    print(circleCollider2D[i].point);
                    
                    Debug.DrawRay(transform.position, -circleCollider2D[i].normal * ((Vector2)transform.position - circleCollider2D[i].point).magnitude, Color.cyan, 2f);
                     circleCollider2D[i].rigidbody.AddForce( (1 - ((Vector2)transform.position - circleCollider2D[i].point).magnitude / slamDist) * slam * slamScalar * circleCollider2D[i].normal,ForceMode2D.Impulse);
                }
            }
        }
    }

    public void EndPowerUp()
    {
        ResetPlayer();
    }
    
}

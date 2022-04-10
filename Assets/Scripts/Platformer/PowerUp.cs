/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private float upDownDist;
    [SerializeField] private float duration;
    public float moveSpeedX;
    [SerializeField] private UnityEvent onCollide = new UnityEvent();
    private PlayerMovement playerMovement;
    private Rigidbody2D _rb;
    private bool used;

    public Vector2 pushForce;
    

    private Vector3 origin;

    // Start is called before the first frame update
    void Awake()
    {
        origin = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        if (upDownDist > 0)
        {
            _rb.gravityScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (used)
            return;
        transform.position += new Vector3(moveSpeedX, origin.y * Mathf.Sin(Time.time) * upDownDist) * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || used)
            return;
        used = true;
        //Will always be true, collectables can only collide with Players in layer.
        //Should only be used in minigame context
        playerMovement = other.gameObject.GetComponent<PlayerMovement>();
        playerMovement.EndPowerUp();
        if(TryGetComponent(out SpriteRenderer component))
            component.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        _rb.simulated = false;
        onCollide.Invoke();
        StartTimer(playerMovement);
    }

    private async void StartTimer(PlayerMovement pm)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            await Task.Yield();
        }
        pm.EndPowerUp();
        Destroy(gameObject);
    }

    public void IncreaseJumpHeight(float val)
    {
        playerMovement._useJumpPower += val;
    }

    public void IncreaseSpeed(float val)
    {
        playerMovement._useSpeed += val;
    }

    public void SlamAbility()
    {
        playerMovement._shouldSlam = true;
    }
}
*/
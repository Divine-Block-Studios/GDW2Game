using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private float upDownDist;
    [SerializeField] private float duration;
    [SerializeField] private float moveSpeedX;
    [SerializeField] private UnityEvent onCollide = new UnityEvent();
    

    private Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        origin = new Vector3(origin.x + moveSpeedX * Time.deltaTime, origin.y, origin.z);
        transform.position = origin + new Vector3(0, Mathf.Sin(Time.time) * upDownDist, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Will always be true, collectables can only collide with Players in layer.
        //Should only be used in minigame context
        PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
        pm.EndPowerUp();
        if(TryGetComponent(out SpriteRenderer component))
            component.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        onCollide.Invoke();
        StartTimer(pm);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(-2f, 0f);
        rb.angularVelocity = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("enter");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("exit");
    }
}

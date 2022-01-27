using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayer : MonoBehaviour
{
    private byte _location;
    private byte _stars;
    public ushort Coins { get; set; }

    [SerializeField] private Text coinsText;

    private void Awake()
    {
        //This is temporary, It should show the coins of the CURRENT player. if the player presses esc, or the "esc" button for IOS they can see all players Icons, balances and names.
        coinsText.text = Coins.ToString();
    }

    // Start is called before the first frame update
    void Update()
    {
        //Allow any player to click, at any point.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green,5 );
                Debug.Log(hit.collider.name);
                if (hit.collider.GetComponent<Interactable>())
                {
                    hit.collider.GetComponent<Interactable>().Pressed();
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 500, Color.red, 5);
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = GameManager.gameManager.CameraArm.rotation;
    }
}

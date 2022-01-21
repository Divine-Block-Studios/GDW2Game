using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    private byte _location;
    private short _coins;
    private byte _stars;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = GameManager.gameManager.CameraArm.rotation;
    }
}

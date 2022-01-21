using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CamRotation : MonoBehaviour
{
    //Imagine a 3D rotation circle.
    //Origin of circle is placed at current player
    [Range(0.01f, 1f)]
    [SerializeField] private float smoothFactor = 0.5f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 _cameraOffset;

    private bool isRotating;

    private Vector3 orbitPoint;
    private Vector3 curPlayerPosition;

    // Start is called before the first frame update
    void Start()
    {
        curPlayerPosition = GameManager.gameManager.GetCurrentPlayer.transform.position;
        _cameraOffset = transform.position - curPlayerPosition;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void MoveCamera()
    {
        //Lerp cam from player to player.
    }

    private void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            Quaternion camTurnAngle =
                Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed,
                    Vector3.up); // * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.up);
            _cameraOffset = camTurnAngle * _cameraOffset;

            transform.LookAt(GameManager.gameManager.GetCurrentPlayer.transform);
            //transform.Rotate(Vector3.forward, 90);
            Vector3 newPos = curPlayerPosition + _cameraOffset;
            transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardInputControls : MonoBehaviour
{
    // These may need to be moved
    [SerializeField] private float sensitivity; 
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float maxAngle;
    public Transform cameraArmBase;
    public Controls _controls;
    public bool canRayCast;
    
    private float _xRot;
    private float _zRot;

    private int count = 0;
    public void Init()
    {
        _controls = new Controls();
        canRayCast = true;
        
        print("Success");
        _controls.PCBoardControls.Interact.started += OnInteract;
        _controls.PCBoardControls.RotateCamera.started += OnRotate;
        _controls.PCBoardControls.Zoom.started += OnZoom;
        #if UNITY_IOS || UNITY_ANDROID
        // Done a bit of "code hacking" to get this to work. 1 tap for a short period triggers an interact. [If the finger is removed, it will make events more clear]
        // Holding the finger down for longer triggers rotate camera. [Acting very similarly to mouse rotate]
        // Then finally, instead of storing a new finger, if the camera is rotating (it must be if there's one finger) and a second finger lands, the player is trying to zoom! 
        _controls.TouchBoardControls.Interact.started += OnInteract;  // Try canceled?
        _controls.TouchBoardControls.Zoom.started += OnZoom;
        _controls.TouchBoardControls.RotateCamera.started += OnRotate;
        #endif


        GameObject.Find("CM vcam1").GetComponent<IntroController>().Init();
        _controls.Enable(); 
    }

    public void OnEnable()
    {
        _controls?.Enable();   
    }

    public void OnDisable()
    {
        _controls?.Disable();
    }

    // Start is called before the first frame update
    private void OnInteract(InputAction.CallbackContext context)
    {
        //Change this, it should cast if a button was not pressed... How?
        if (canRayCast)
        {
            #if UNITY_IOS || UNITY_ANDROID
            if (_controls.TouchBoardControls.RotateCamera.WasReleasedThisFrame())
            {
                return;
            }

            
            #endif
            Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit hit, 500))
            {
                GameObject go = new GameObject();
                LineRenderer lr = go.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, hit.point);
                lr.startColor = Color.green;
                lr.endColor = Color.green;
                lr.startWidth = 0.01f;
                lr.endWidth = 0.01f;
                Destroy(go, 5);
                if (hit.collider.GetComponent<Interactable>())
                {
                    hit.collider.GetComponent<Interactable>().Pressed();
                }
            }
            else
            {
                GameObject go = new GameObject();
                Instantiate(go);
                LineRenderer lr = go.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, ray.origin);
                lr.SetPosition(1, ray.direction * 500);
                lr.startColor = Color.red;
                lr.startWidth = 0.01f;
                lr.endWidth = 0.01f;
                lr.endColor = Color.red;
                Destroy(go, 5);
            }
        }
    }
    private void OnZoom(InputAction.CallbackContext context)
    {
        if (!GameManager.gameManager.isEnabled)
            return;
        Vector2 zoomInput = context.ReadValue<Vector2>();

        Vector3 position = cameraArmBase.GetChild(0).GetChild(0).localPosition;
        position.z = Mathf.Clamp(position.z + (Time.deltaTime * scrollSensitivity * zoomInput.y), minSize, maxSize);
        cameraArmBase.GetChild(0).GetChild(0).localPosition = position;
    }


    private void OnRotate(InputAction.CallbackContext context)
    {
        if (!GameManager.gameManager.isEnabled)
            return;
        Vector2 rotateInfo = context.ReadValue<Vector2>();
        
        _zRot -= rotateInfo.x * sensitivity / 1000;
        _xRot -= rotateInfo.y * sensitivity / 1000;

        _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);
        
        cameraArmBase.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
        cameraArmBase.localRotation = Quaternion.Euler(0,0,_zRot);
    }
}

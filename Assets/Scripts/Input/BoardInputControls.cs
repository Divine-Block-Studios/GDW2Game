using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class BoardInputControls : MonoBehaviour
{
    // These may need to be moved
    [SerializeField] private float sensitivity; 
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float maxAngle;
    [SerializeField] private Transform cameraArmBase;
    public Text debugText;
    private Controls _controls;
    
    private float _xRot;
    private float _zRot;

    private int count = 0;
    private void Awake()
    {
        _controls = new Controls();
        if (!debugText)
        {
            debugText = GameObject.Find("DEBUGTEXT").GetComponent<Text>();
        }
    }

    private void OnEnable()
    {
        _controls.Enable();   
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Start()
    {
        print("Success");
        //#if UNITY_STANDALONE
        _controls.PCBoardControls.Interact.started += OnInteract;
        _controls.PCBoardControls.RotateCamera.started += OnRotate;
        _controls.PCBoardControls.Zoom.started += OnZoom;
        //#elif UNITY_IOS || UNITY_ANDROID
        // Done a bit of "code hacking" to get this to work. 1 tap for a short period triggers an interact. [If the finger is removed, it will make events more clear]
        // Holding the finger down for longer triggers rotate camera. [Acting very similarly to mouse rotate]
        // Then finally, instead of storing a new finger, if the camera is rotating (it must be if there's one finger) and a second finger lands, the player is trying to zoom! 
        _controls.TouchBoardControls.Interact.started += OnInteract;  // Try canceled?
        _controls.TouchBoardControls.Interact.started += (ctx) =>
        {
            debugText.text = "Interact Started";
        };
        _controls.TouchBoardControls.Interact.performed += (ctx) =>
        {
            debugText.text = "Interact performed";
        };
        _controls.TouchBoardControls.Interact.canceled += (ctx) =>
        {
            debugText.text = "Interact canceled";
        };
        _controls.TouchBoardControls.Zoom.started += OnZoom;
        _controls.TouchBoardControls.RotateCamera.started += OnRotate;
        //#endif
    }

    


    // Start is called before the first frame update
    private void OnInteract(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay (context.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 500))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green,5 );
            if (hit.collider.GetComponent<Interactable>())
            {
                hit.collider.GetComponent<Interactable>().Pressed();
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 500, Color.red, 5);
        }
        debugText.text = "RayCasted " + ++count;
    }
    private void OnZoom(InputAction.CallbackContext context)
    {
        Vector2 zoomInput = context.ReadValue<Vector2>();

        Vector3 position = cameraArmBase.GetChild(0).GetChild(0).localPosition;
        position.z = Mathf.Clamp(position.z + (Time.deltaTime * scrollSensitivity * zoomInput.y), minSize, maxSize);
        cameraArmBase.GetChild(0).GetChild(0).localPosition = position;

        debugText.text = "Zooming: " + position.z + " - (input): " + zoomInput;
    }


    private void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 rotateInfo = context.ReadValue<Vector2>();
        
        _zRot -= rotateInfo.x * sensitivity / 1000;
        _xRot -= rotateInfo.y * sensitivity / 1000;

        _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);
        
        cameraArmBase.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
        cameraArmBase.localRotation = Quaternion.Euler(0,0,_zRot);
        //debugText.text = "Rotating: " + cameraArmBase.eulerAngles.z + " -  (input) " + rotateInfo;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamRotation : MonoBehaviour
{
    
    [SerializeField] private float sensitivity;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float maxAngle;
    
    private float _xRot;
    private float _zRot;

    private float _distance;

    private Controls _controls;
    private Coroutine zoomCoroutine;

    private void Awake()
    {
        _controls = new Controls();
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
       //_controls.CoreGameControls.Zoom.started += _ => ZoomStart();
        //_controls.CoreGameControls.Zoom.canceled += _ => ZoomEnd();
        //_distance = transform.GetChild(0).GetChild(0).localPosition.z;
    }

    private void Update()
    {
        #if UNITY_STANDALONE
            if (Input.GetKey(KeyCode.Mouse1))
                Rotate();
        #elif UNITY_IOS || UNITY_ANDROID
            
        #endif
    }
    
    private void Rotate()
    {
        //Run tests to see if this if statement is faster here, or faster in start paired with a boolean
        #if UNITY_STANDALONE //directive for compiling/executing code for any standalone platform (Mac OS X, Windows or Linux).
            float mx = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float my = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            float ms = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

            //See in editor. Rotating horizontally affects Y and vice versa
            _zRot -= mx;
            _xRot -= my;

            _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);

            _distance = Mathf.Clamp(ms + _distance, -maxSize, -minSize);

            transform.GetChild(0).GetChild(0).localPosition = new Vector3(0,0,_distance);
            transform.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
            transform.rotation = Quaternion.Euler(0,0,_zRot);
        #elif UNITY_IOS || UNITY_ANDROID// directive for compiling/executing code for the iOS platform. Android for andriod. Select in build setting to use.
            
        #endif
    }

    #if UNITY_IOS || UNITY_ANDROID
    private void ZoomStart()
    {
        //print("ZoomStarting: " + _controls.CoreGameControls.Zoom.bindings[0].);
    }

    private void ZoomEnd()
    {
        print("ZoomEnd");
        StopCoroutine(zoomCoroutine);
    }
#endif 

    private IEnumerator ZoomDetection()
    {
        #if UNITY_STANDALONE
        #elif UNITY_IOS || UNITY_ANDROID
        float previousDistance = 0f;
        float distance = 0f;
        while (true)
        {
            //distance = Vector2.Distance(_controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), _controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

            //Zooming out
            if (distance > previousDistance)
            {
                Vector3 position = transform.position;
                Vector3 targetPos = position;
                targetPos.z -= 1;
                position = Vector3.Slerp(position, targetPos, Time.deltaTime * scrollSensitivity);
                transform.position = position;
            }
            //Zooming in
            else if (distance < previousDistance)
            {
                Vector3 position = transform.position;
                Vector3 targetPos = position;
                targetPos.z += 1;
                position = Vector3.Slerp(position, targetPos, Time.deltaTime * scrollSensitivity);
                transform.position = position;
            }
            previousDistance = distance;
            yield return null;
        }
#endif
        yield return null;
    }

    private void PanCam()
    {
        #if UNITY_STANDALONE
        #elif UNITY_IOS || UNITY_ANDROID
        #endif
    }
    
    /*
     * #if UNITY_STANDALONE
        #elif UNITY_IOS || UNITY_ANDROID
        #endif
     */

    public void OnZoom(InputAction.CallbackContext context)
    {
        Vector2 zoomInput = context.ReadValue<Vector2>();
        print("Zooming: " + zoomInput);
        
        Vector3 position = transform.GetChild(0).GetChild(0).localPosition;
        position.z = Mathf.Clamp(position.z + (Time.deltaTime * scrollSensitivity * zoomInput.y), minSize, maxSize);
        transform.GetChild(0).GetChild(0).localPosition = position;
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        print("Raycasting");
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 rotateInfo = context.ReadValue<Vector2>();
        print("rotating: " + rotateInfo);
        
        _zRot -= rotateInfo.x * sensitivity / 1000;
        _xRot -= rotateInfo.y * sensitivity / 1000;

        _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);
        
        transform.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
        transform.rotation = Quaternion.Euler(0,0,_zRot);
    }
}
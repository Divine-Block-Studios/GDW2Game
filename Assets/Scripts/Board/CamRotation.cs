using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class CamRotation : MonoBehaviour
{
    
    [SerializeField] private float sensitivity;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float maxAngle;
    
    private float _xRot;
    private float _zRot;

    private float distance;

    private void Start()
    {
        distance = transform.GetChild(0).GetChild(0).localPosition.z;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            Rotate();
    }

    
    
    
    
    private void Rotate()
    {
        //Run tests to see if this if statement is faster here, or faster in start paired with a boolean
        #if UNITY_STANDALONE //directive for compiling/executing code for any standalone platform (Mac OS X, Windows or Linux).
            print("rotate");
            float mx = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float my = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            float ms = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

            //See in editor. Rotating horizontally affects Y and vice versa
            _zRot -= mx;
            _xRot -= my;

            _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);

            distance = Mathf.Clamp(ms + distance, -maxSize, -minSize);
            print(distance);

            transform.GetChild(0).GetChild(0).localPosition = new Vector3(0,0,distance);
            transform.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
            transform.rotation = Quaternion.Euler(0,0,_zRot);
        #elif UNITY_IOS || UNITY_ANDROID// directive for compiling/executing code for the iOS platform. Android for andriod. Select in build setting to use.
            
        #endif
    }
}
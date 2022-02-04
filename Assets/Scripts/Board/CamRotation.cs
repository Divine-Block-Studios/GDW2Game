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


}
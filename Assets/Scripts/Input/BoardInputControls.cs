using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardInputControls : MonoBehaviour
{
    // These may need to be moved
    [SerializeField] private float sensitivity; 
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float maxAngle;
    [SerializeField] private Transform cameraArmBase;
    
    
    private Controls _controls;
    
    private float _xRot;
    private float _zRot;

    private void Awake()
    {
        _controls = new Controls();
    }
    
    
    // Start is called before the first frame update
    public void OnZoom(InputAction.CallbackContext context)
    {
        Vector2 zoomInput = context.ReadValue<Vector2>();
        
        Vector3 position = cameraArmBase.GetChild(0).GetChild(0).localPosition;
        position.z = Mathf.Clamp(position.z + (Time.deltaTime * scrollSensitivity * zoomInput.y), minSize, maxSize);
        cameraArmBase.GetChild(0).GetChild(0).localPosition = position;
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started /*&& GameManager.gameManager.GetCurrentPlayer.InMenu*/)
        {
            Ray ray = Camera.main.ScreenPointToRay (Mouse.current.position.ReadValue());
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
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 rotateInfo = context.ReadValue<Vector2>();
        
        _zRot -= rotateInfo.x * sensitivity / 1000;
        _xRot -= rotateInfo.y * sensitivity / 1000;

        _xRot = Mathf.Clamp(_xRot, -maxAngle, 0);
        
        cameraArmBase.GetChild(0).localRotation = Quaternion.Euler(_xRot,0,0);
        cameraArmBase.rotation = Quaternion.Euler(0,0,_zRot);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class SamuraiController : MonoBehaviour
{
    [SerializeField] private int ptsCount;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask cheeseLayer;
    [SerializeField] private CheeseSamurai gameController;
    public int playerIndex;
    
    private Queue<Vector3> points = new Queue<Vector3>();
    private LineRenderer _lineRenderer;

    private float z;

    public int curScore;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = ptsCount;
        _lineRenderer.alignment = LineAlignment.TransformZ;
        z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 50, cheeseLayer))
        {
            float force = (_lineRenderer.GetPosition(ptsCount - 2) - _lineRenderer.GetPosition(ptsCount - 3)).magnitude;
            if (force < 0.015f)
                return;
            Cheese cheese = hit.collider.GetComponent<Cheese>();
            Vector2 direction = (_lineRenderer.GetPosition(ptsCount-2) - _lineRenderer.GetPosition(ptsCount-3)).normalized;
            
            print("has cheese: " + cheese);
            print("Force: " + force);
            print("Direction: " + direction);
            cheese.Cut(direction, force);
            curScore += cheese.value;
            gameController.myScoreText.text = curScore.ToString();
            gameController.UpdateScore(playerIndex, curScore);
        }
        
        Vector3 pt = Mouse.current.position.ReadValue();
        pt.z = -z;
        pt = cam.ScreenToWorldPoint(pt);
        pt.z = z;
        
        points.Enqueue(pt);
        if (points.Count > ptsCount)
        {
            points.Dequeue();
        }
        _lineRenderer.SetPositions(points.ToArray());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DiceAnim : MonoBehaviour
{
    [SerializeField] private float maxRot;
    [SerializeField] private float rotSpeed;
    
    private bool isSpinning;
    private void OnEnable()
    {
        isSpinning = true;
        Spin();
    }

    private void OnDisable()
    {
        isSpinning = false;
    }

    // Update is called once per frame
    private async void Spin()
    {
        float dist = 0;
        while (isSpinning)
        {
            transform.eulerAngles += new Vector3(0, 0, rotSpeed);

            dist += rotSpeed;
            
            //Hit right boundry OR Hit left boundry
            if (dist < -maxRot || dist > maxRot)
            {
                rotSpeed *= -1;
            }

            await Task.Yield();
        }
    }
}

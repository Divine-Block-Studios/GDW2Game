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
        while (isSpinning)
        {
            transform.eulerAngles += new Vector3(0, 0, rotSpeed);
            //Hit right boundry OR Hit left boundry
            if (transform.eulerAngles.z >= maxRot || transform.eulerAngles.z <= 360 - maxRot)
            {
                maxRot *= -1;
                rotSpeed *= -1;
            }

            await Task.Yield();
        }
    }
}

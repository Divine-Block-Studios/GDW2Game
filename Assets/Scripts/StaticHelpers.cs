using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StaticHelpers
{
    public static async void MoveLerp(Transform obj, Vector3 start, Vector3 moveTo, float moveSpeed,
        Action uponCompletion = null, bool animated = false)
    {
        float travelTime = Vector3.Distance(start, moveTo) / (moveSpeed * Time.deltaTime);
        float curTime = 0f;
        while (curTime < travelTime)
        {
            obj.position = Vector3.Lerp(start, moveTo, curTime / travelTime);
            curTime += moveSpeed * Time.deltaTime;
            await Task.Yield();
        }

        //Trigger next tile
        if (uponCompletion != null)
            uponCompletion.Invoke();
        await Task.Yield();
    }

    //Move to general use function. On compelete do something.
    public static async void MoveSlerp(Transform obj, Vector3 start, Vector3 moveTo, float moveSpeed,
        Action uponCompletion = null)
    {
        //TODO: fix, this is not true. Need to do some sort of circle math, angle is always 180? Multiply by smth,
        float travelTime = Vector3.Distance(start, moveTo) / (moveSpeed * Time.deltaTime);
        float curTime = 0f;
        while (curTime < travelTime)
        {
            obj.position = Vector3.Slerp(start, moveTo, curTime / travelTime);
            curTime += moveSpeed * Time.deltaTime;
            await Task.Yield();
        }

        //Trigger next tile
        if (uponCompletion != null)
            uponCompletion.Invoke();
        await Task.Yield();
    }

    public static void ThrowAt(Transform obj, Vector3 start, Vector3 end, float throwRngAngle, float spinAngleRng, float speed)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        end = Quaternion.AxisAngle(-Vector3.forward , Random.Range(-throwRngAngle, throwRngAngle)) * end-start;
        
        Debug.Log((end-start).normalized);
        Debug.DrawRay(start, end, Color.black, 20);
        rb.AddTorque(Random.Range(-spinAngleRng, spinAngleRng), Random.Range(-spinAngleRng, spinAngleRng), Random.Range(-spinAngleRng, spinAngleRng));
        rb.AddForce(end * speed, ForceMode.Impulse);
    }
    
    //Custom Variation of FisherYates array shuffle method.
    public static void Shuffle <T>(T [] array)
    {
        int length = array.Length;
        while(length > 1)
        {
            //Set RNG to be a random element in the array (Excluding those which have been modified)
            int rng = Random.Range(0, length--);

            //Set temp to be last element
            T temp = array[length];
            //Set last element to be the random selected element
            array[length] = array[rng];
            //Set Random element to be what the old last element was
            array[rng] = temp;
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public static class StaticHelpers
{
    public static async void MoveLerp(Transform obj, Vector3 start, Vector3 moveTo, float moveSpeed,
        Action uponCompletion = null, bool animated = false)
    {
        //T = D/V
        float travelDist = Vector3.Distance(start, moveTo);
        float curDist = 0f;
        while (curDist < travelDist)
        {
            obj.position = Vector3.Lerp(start, moveTo, curDist / travelDist);
            curDist += moveSpeed * Time.deltaTime;
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
        float travelDist = Vector3.Distance(start, moveTo) * 1.5f;
        float curDist = 0f;
        while (curDist < travelDist)
        {
            obj.position = Vector3.Slerp(start, moveTo, curDist / travelDist);
            curDist += moveSpeed * Time.deltaTime;
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
    
    public static void ThrowAt2D(Transform obj, Vector3 start, Vector3 end, float throwRngAngle, float spinAngleRng, float speed)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        end = Quaternion.AxisAngle(-Vector3.forward , Random.Range(-throwRngAngle, throwRngAngle)) * end-start;
        
        Debug.Log((end-start).normalized);
        Debug.DrawRay(start, end, Color.black, 20);
        rb.AddForce(end * speed);
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

    public static void Swap<T>(ref T a, ref T b)
    {
        T c = a;
        a = b;
        b = c;
    }

    public static void StartFrom<T>(ref T[] arr, int index)
    {
        //-1 cus we don't need to swap last index. makes no sense.
        for (int i = 0; i < arr.Length-1; i++)
        {
            //0,1,2,3,4,5,6 (init)
            //3,1,2,0,4,5,6 (index = 3, i = 0)
            //3,4,2,0,1,5,6 (index = 4, i = 1)
            //3,4,5,0,1,2,6 (index = 5, i = 2)
            //3,4,5,6,1,2,0 (index = 6, i = 3)
            //3,4,5,6,0,2,1 (index = CAPPED (6), i = 4)
            //3,4,5,6,0,1,2 (index = CAPPED (6), i = 5)
            
            Swap(ref arr[Mathf.Min(index++, arr.Length-1)], ref arr[i]);
        }
    }

    public static async void Fade(Image myColor, Color lerpTo, float seconds, float delay)
    {
        await Task.Delay((int)(delay*1000));
        float curSeconds = 0;
        Color origin = myColor.tintColor;
        while (curSeconds < seconds)
        {
            myColor.tintColor = Color.Lerp(origin, lerpTo, curSeconds/seconds);
            curSeconds += Time.deltaTime;
            await Task.Yield();
        }
    }
    
    public static async void Fade(SpriteRenderer myColor, Color lerpTo, float seconds, float delay)
    {
        await Task.Delay((int)(delay*1000));
        float curSeconds = 0;
        Color origin = myColor.color;
        while (curSeconds < seconds)
        {
            myColor.color = Color.Lerp(origin, lerpTo, curSeconds/seconds);
            curSeconds += Time.deltaTime;
            await Task.Yield();
        }
    }
    
    public static async void Fade(TextMeshPro myColor, Color lerpTo, float seconds, float delay)
    {
        await Task.Delay((int)(delay*1000));
        float curSeconds = 0;
        Color origin = myColor.color;
        while (curSeconds < seconds)
        {
            myColor.color = Color.Lerp(origin, lerpTo, curSeconds/seconds);
            curSeconds += Time.deltaTime;
            await Task.Yield();
        }
    }
    
    public static async void Fade(TextMeshProUGUI myColor, Color lerpTo, float seconds, float delay)
    {
        Debug.Log("Waiting: " + (int)(delay*1000));
        await Task.Delay((int)(delay*1000));
        Debug.Log("Beginning Fade");
        float curSeconds = 0;
        Color origin = myColor.color;
        while (curSeconds < seconds)
        {
            myColor.color = Color.Lerp(origin, lerpTo, curSeconds/seconds);
            curSeconds += Time.deltaTime;
            await Task.Yield();
        }
    }

    private static bool _curtainAreOpen = true;

    public static async void Curtains(Action onComplete, float delay = 0)
    {
        _curtainAreOpen = !_curtainAreOpen;

        GameObject curtainsHolder = GameObject.FindWithTag("Curtains");

        if (!curtainsHolder)
        {
            Debug.LogError("FATAL: no GameObject is marked with curtains tag.");
        }

        curtainsHolder.GetComponent<Canvas>().sortingOrder = 7;
        await Task.Delay((int) (delay * 1000));
        Animator anim = curtainsHolder.transform.GetChild(0).GetComponent<Animator>();

        anim.SetBool("IsOpen", _curtainAreOpen);
        await Task.Delay(1000);
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            Debug.Log("Test:" + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            await Task.Yield();
        }
        curtainsHolder.GetComponent<Canvas>().sortingOrder = -7;
        
        onComplete?.Invoke();
    }

    public static async void MuteAudio(AudioSource a, float b, float time, float delay = 0)
    {
        await Task.Delay((int)(delay * 1000));

        float curTime = 0;
        float startPos = a.volume;
        while (curTime < time)
        {
            a.volume= Mathf.Lerp(startPos, b, curTime / time);
            curTime += Time.deltaTime;
        }
    }

}

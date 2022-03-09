using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class StaticHelpers
{
    public static async void MoveLerp(Transform obj, Vector3 start, Vector3 moveTo, float moveSpeed, Action uponCompletion = null, bool animated = false)
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
        if(uponCompletion != null)
            uponCompletion.Invoke();
        await Task.Yield();
    }

    //Move to general use function. On compelete do something.
    public static async void MoveSlerp(Transform obj, Vector3 start, Vector3 moveTo, float moveSpeed, Action uponCompletion = null)
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
        if(uponCompletion != null)
            uponCompletion.Invoke();
        await Task.Yield();
    }
}

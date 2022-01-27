using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlatformSpecificAssets : MonoBehaviour
{
    // Disable the object if it's not meant for the current console
    [SerializeField] private bool forMobileDevices;
    [SerializeField] private Text text;
    [SerializeField] private string pcWords;
    [SerializeField] private string mobileWords;
    void Start()
    {
        #if UNITY_STANDALONE
        if (text)
        {
            text.text = pcWords;
        }
        if (forMobileDevices)
        {
            gameObject.SetActive(false);
        }
        #elif UNITY_IOS || UNITY_ANDROID
        if (text)
        {
            text.text = mobileWords;
        }
        if (!forMobileDevices)
        {
            gameObject.SetActive(false);
        } 
        #endif
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonMono<T> : MonoBehaviour where T : MonoBehaviour
{
   // public static bool applicationIsQuitting = false;

    private static T _instance;

    private static object _lock = new object();

    public static T GetInstance
    {
        get
        {
            // if (applicationIsQuitting)
            // {
            //     Debug.Log("[Singleton] Instance" + typeof(T) + "already destroyed on application quit." +
            //               "Won't create again-returning null");
            //     return null;
            // }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T) FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject singleTonMono = new GameObject();
                        _instance = singleTonMono.AddComponent<T>();
                        singleTonMono.name = "(singleTonMono)" + typeof(T).ToString();
                    }
                }
            }

            return _instance;
        }
    }


    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        //applicationIsQuitting = true;
        UnInit();
    }

    protected virtual void Init()
    {
        //TODO 目前不是单例，XR切场景时有问题，后续改下
     //   DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void UnInit()
    {
    }
}
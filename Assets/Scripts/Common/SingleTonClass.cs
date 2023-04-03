using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonClass<T> where T : class, new()
{
    private static T _instance;

    public static T GetInstance()
    {
        if (_instance == null)
        {
            CreateInstance();
        }

        return _instance;
    }

    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = Activator.CreateInstance<T>();
            (_instance as SingleTonClass<T>).Init();
        }
    }

    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            (_instance as SingleTonClass<T>).UnInit();
        }

        _instance = null;
    }

    protected virtual void Init()
    {
    }

    protected virtual void UnInit()
    {
    }
}
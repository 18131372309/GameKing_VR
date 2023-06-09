using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[Serializable]
public class Button3DClickEventRegister : UnityEvent
{
}

[Serializable]
public class Button3DEnterEventRegister : UnityEvent
{
}

[Serializable]
public class Button3DStayEventRegister : UnityEvent
{
}

[Serializable]
public class Button3DExitEventRegister : UnityEvent
{
}

public enum Button3DShowStyle
{
    ColorTint
}

//TODO:结合射线，实现整套输入系统的封装
[RequireComponent(typeof(Button3DRaycaster))]
public class Button3DCustom : MonoBehaviour
{
    public bool interactable = true;
    public Button3DShowStyle transition;

    //TODO:GUI Inspector修改样式
    public Color defaultColor = Color.white;
    public Color enterColor = Color.white;
    public Color highlightColor = Color.white;
    public Color exitColor = Color.white;

    [Space] [Space] [Space] [SerializeField]
    private Button3DClickEventRegister onClick;

    [SerializeField] private Button3DEnterEventRegister onEnter;
    [SerializeField] private Button3DStayEventRegister onStay;
    [SerializeField] private Button3DExitEventRegister onExit;

    // private UnityAction ClickAction;
    // private UnityAction EnterAction;
    // private UnityAction StayAction;
    // private UnityAction ExitAction;

    private Transform buttonBg;
    private Material bgMat;

    private void Start()
    {
        InitData();
        InitState();
        //   InitEvent();
    }

    private void Update()
    {
    }

    void InitData()
    {
        buttonBg = gameObject.transform;
        bgMat = buttonBg.GetComponent<MeshRenderer>().material;
    }

    void InitState()
    {
        bgMat.color = defaultColor;
    }

    void InitEvent()
    {
        // onClick.AddListener(ClickAction);
        // EnterAction += onEnter.onRayEnterAction;
        // StayAction += onStay.onRayStayAction;
        // ExitAction += onExit.onRayExitAction;
    }

    public void OnClick()
    {
        // ClickAction?.Invoke();
        onClick?.Invoke();
    }

    public void AddListener(UnityAction handle)
    {
        // ClickAction += handle;
        onClick.AddListener(handle);
    }

    public void OnRayEnter()
    {
        // if (EnterAction != null)
        // {
        //     EnterAction?.Invoke();
        // }

        if (onEnter != null)
        {
            onEnter?.Invoke();
        }

        if (bgMat.color != enterColor)
        {
            bgMat.color = enterColor;
        }
    }

    public void OnRayStay()
    {
        // if (StayAction != null)
        // {
        //     StayAction?.Invoke();
        // }

        if (onStay != null)
        {
            onStay?.Invoke();
        }

        if (bgMat.color != highlightColor)
        {
            bgMat.color = highlightColor;
        }
    }

    public void OnRayExit()
    {
        // if (ExitAction != null)
        // {
        //     ExitAction?.Invoke();
        // }

        if (onExit != null)
        {
            onExit?.Invoke();
        }

        if (bgMat.color != exitColor)
        {
            bgMat.color = exitColor;
        }
    }

    public void RemoveAllListener()
    {
        // ClickAction = null; //只移除click事件
        onClick.RemoveAllListeners();
    }

    public void OnDestroy()
    {
        // ClickAction = null;
        // EnterAction = null;
        // StayAction = null;
        // ExitAction = null;
        onClick.RemoveAllListeners();
        onEnter.RemoveAllListeners();
        onStay.RemoveAllListeners();
        onExit.RemoveAllListeners();
    }
}
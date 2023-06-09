using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public enum ViewMode
{
    Ground,
    Sky,
    SandBox
}

public class XROriginInstance : SingleTonMono<XROriginInstance>
{
    public static GameObject XROriginObj;
    public static ViewMode curViewMode;

    public static GameObject leftControllerObj;
    public static GameObject rightControllerObj;
    
    // Start is called before the first frame update
    void Awake()
    {
        XROriginObj = this.gameObject;
        curViewMode = ViewMode.Ground;

        leftControllerObj = XROriginObj.transform.Find("Camera Offset/LeftHand Controller").gameObject;
        rightControllerObj = XROriginObj.transform.Find("Camera Offset/RightHand Controller").gameObject;
        
    }

    // private void Start()
    // {
    //     StartCoroutine(CheckModelBorn());
    // }
    //
    // IEnumerator CheckModelBorn()
    // {
    //     yield return new WaitUntil(() =>
    //     {
    //         return leftControllerObj.GetComponent<XRController>().model != null &&
    //                rightControllerObj.GetComponent<XRController>().model != null;
    //     });
    //     
    //     EventDispatcher.GetInstance().DispatchEvent("handleModel");
    // }
}
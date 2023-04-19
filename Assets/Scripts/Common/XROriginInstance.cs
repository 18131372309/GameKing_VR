using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    
    // Start is called before the first frame update
    void Awake()
    {
        XROriginObj = this.gameObject;
        curViewMode = ViewMode.Ground;
    }
}
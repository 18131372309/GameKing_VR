using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XROriginInstance : SingleTonMono<XROriginInstance>
{
    public static GameObject XROriginObj;

    // Start is called before the first frame update
    void Awake()
    {
        XROriginObj = this.gameObject;
    }
}
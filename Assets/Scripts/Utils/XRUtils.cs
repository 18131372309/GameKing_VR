﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.Qiyu;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUtils : SingleTonClass<XRUtils>
{
    private GameObject leftHandleModel;
    private GameObject rightHandleModel;

    /// <summary>
    /// 手柄震动
    /// </summary>
    /// <param name="inputDevice">输入设备</param>
    /// <param name="channel">频道</param>
    /// <param name="amplitude">强度</param>
    /// <param name="duration">持续时间</param>
    public void SendHapticImpulse(InputDevice inputDevice, uint channel, int amplitude, float duration)
    {
        inputDevice.SendHapticImpulse(channel, amplitude, duration);
    }

    /// <summary>
    /// MR模式切换
    /// </summary>
    /// <param name="visible"></param>
    public void SetPassThrough(bool visible)
    {
        var deviceInfo = Unity.XR.Qiyu.QiyuXRCore.GetDeviceInfo();
        if (deviceInfo.Name_HMD != "QiyuHMD-3" && deviceInfo.Name_HMD != "QiyuHMD-Dream" &&
            deviceInfo.Name_HMD != "QiyuHMD-Dream Pro")
        {
            //passthrough现阶段只支持MIX及以后新机型，需要排除老机型后开启，否则开启后是黑的。
            QiyuSeeThrough.EnableSeeThrough(visible);
        }
    }

    /// <summary>
    /// 设置手柄模型隐藏
    /// </summary>
    /// <param name="visible"></param>
    public void SetHandleModelState(bool visible)
    {
        GameObject.Find("LeftHand Controller").GetComponent<XRController>().hideControllerModel =
            !visible;
        GameObject.Find("RightHand Controller").GetComponent<XRController>().hideControllerModel =
            !visible;

        // if (leftHandleModel == null || rightHandleModel == null)
        // {
        //     leftHandleModel = GameObject.Find("LeftHand Controller").GetComponent<XRController>().model.gameObject;
        //     rightHandleModel = GameObject.Find("RightHand Controller").GetComponent<XRController>().model.gameObject;
        // }
        //
        // leftHandleModel.SetActive(visible);
        // rightHandleModel.SetActive(visible);
    }
}
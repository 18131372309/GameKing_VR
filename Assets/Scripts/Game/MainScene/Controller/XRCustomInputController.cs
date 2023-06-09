using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class XRCustomInputController : SingleTonMono<XRCustomInputController>
{
    private InputDevice leftController;
    private InputDevice rightController;

    private bool leftGrip = false;
    private bool rightGrip = false;

    private float leftGripValue = 0;
    private float rightGripValue = 0;

    private bool leftTrigger = false;
    private bool rightTrigger = false;

    private bool leftFirstKey = false;
    private int leftFirstKeyTimes = 0;
    private bool leftSecondKey = false;
    private int leftSecondKeyTimes = 0;

    private bool rightFirstKey = false;
    private int rightFirstKeyTimes = 0;

    private bool rightSecondKey = false;
    private int rightSecondKeyTimes = 0;

    private Vector2 right2DAxis;

    private bool isAxisForwordUp = false;

    // private bool rightAxisForwordDown = false;
    //
    // private int rightAxisForwordDowmTimes = 0;
    //
    // private bool rightAxisForwordUp = false;
    //
    // private int rightAxisForwordUpTimes = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitDevices();
        // XRSceneManager.GetInstance.cs += ChangeSceneInit;
    }

    void ChangeSceneInit()
    {
        InitDevices();
        //Debug.Log("ChangeSceneInit");
    }

    //手柄掉綫重連 
    void InitDevices()
    {
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevices.deviceConnected += RegisterDevices;
    }

    void RegisterDevices(InputDevice connectDevice)
    {
        XRDebug.Log(connectDevice.name);
        if (connectDevice.isValid)
        {
            if ((connectDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
                XRDebug.Log("leftController is On");
                leftController = connectDevice;
            }

            if ((connectDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                XRDebug.Log("rightController is On");
                rightController = connectDevice;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        XrKeyDownCheck();
    }

    //TODO 输入系统封装
    //所有按键的检测
    private void XrKeyDownCheck()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out right2DAxis))
        {
            // XRDebug.Log(right2DAxis.y.ToString());
            if (right2DAxis.y > 0f)
            {
                isAxisForwordUp = false;
                XRTeleportController.GetInstance.OnEnterTeleport();
            }
            else
            {
                if (!isAxisForwordUp)
                {
                    XRTeleportController.GetInstance.OnExitTeleport();
                    isAxisForwordUp = true;
                }
            }
        }

        if (leftController.TryGetFeatureValue(CommonUsages.grip, out leftGripValue) && leftGripValue > 0)
        {
            // XRDebug.Log("L-Grip" + leftGripValue);
        }

        if (rightController.TryGetFeatureValue(CommonUsages.grip, out rightGripValue) && rightGripValue > 0)
        {
            //  XRDebug.Log("R-Grip" + rightGripValue);
            // if (rightGripValue >= 0.9f)
            // {
            //     isAxisForwordUp = false;
            //     XRTeleportController.GetInstance.OnEnterTeleport();
            // }else
            // {
            //     if (!isAxisForwordUp)
            //     {
            //         XRTeleportController.GetInstance.OnExitTeleport();
            //         isAxisForwordUp = true;
            //     }
            //     
            // }
        }

        if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip) && leftGrip)
        {
            //  XRDebug.Log("L-Grip:" + leftGrip);
        }

        if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip) && rightGrip)
        {
            //   XRDebug.Log("R-Grip:" + rightGrip);
        }

        if (leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftTrigger) && leftTrigger)
        {
            // XRDebug.Log("L-Trigger:" + leftTrigger);
        }

        if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTrigger) && rightTrigger)
        {
            // XRDebug.Log("R-Trigger:" + rightTrigger);
        }

        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out leftFirstKey))
        {
            //  XRDebug.Log("L-primaryButton:" + leftFirstKey);
            //TODO 临时展示FPS

            if (leftFirstKey)
            {
                leftFirstKeyTimes++;
                if (leftFirstKeyTimes == 1)
                {
                    GameObject fps = XROriginInstance.XROriginObj.transform
                        .Find("Camera Offset/Main Camera/XRFollowCanvas/MenuPanel/FPSText").gameObject;
                    if (fps.activeInHierarchy)
                    {
                        fps.SetActive(false);
                    }
                    else
                    {
                        fps.SetActive(true);
                    }
                }
            }
            else
            {
                if (leftFirstKeyTimes != 0)
                {
                    leftFirstKeyTimes = 0;
                }
            }
        }

        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out rightFirstKey))
        {
            if (rightFirstKey)
            {
                rightFirstKeyTimes++;
                if (rightFirstKeyTimes == 1)
                {
                    if (XROriginInstance.curViewMode != ViewMode.SandBox)
                    {
                        UIManager.GetInstance.ShowViewModeSelectUi();
                    }
                }
            }
            else
            {
                if (rightFirstKeyTimes != 0)
                {
                    rightFirstKeyTimes = 0;
                }
            }

            // XRDebug.Log("R-primaryButton:" + rightFirstKey);
        }

        if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out leftSecondKey))
        {
            //  XRDebug.Log("L-secondaryButton:" + leftSecondKey);


            if (leftSecondKey)
            {
                leftSecondKeyTimes++;
                if (leftSecondKeyTimes == 1)
                {
                    // //TODO 测试下坠感
                    // if (XROriginInstance.XROriginObj.GetComponent<Rigidbody>() != null)
                    // {
                    //     GameObject.Destroy(XROriginInstance.XROriginObj.GetComponent<Rigidbody>());
                    // }
                    // else
                    // {
                    //     XROriginInstance.XROriginObj.AddComponent<Rigidbody>();
                    // }
                    //  XRDebug.Log("change_sandBoxMode");
                    //  EventDispatcher.GetInstance().DispatchEvent("change_sandBoxMode");
                }
            }
            else
            {
                if (leftSecondKeyTimes != 0)
                {
                    leftSecondKeyTimes = 0;
                }
            }
        }

        if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out rightSecondKey) &&
            rightSecondKey)
        {
            //  XRDebug.Log("R-secondaryButton:" + rightSecondKeyDown);
            if (rightSecondKey)
            {
                rightSecondKeyTimes++;
                if (rightSecondKeyTimes == 1)
                {
                    // XRSceneManager.GetInstance.TeleportDifferentScene("EnterScene");
                }
            }
            else
            {
                if (rightSecondKeyTimes != 0)
                {
                    rightSecondKeyTimes = 0;
                }
            }
        }
    }
}
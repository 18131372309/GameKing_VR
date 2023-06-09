using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class XRCustomRaycastController : SingleTonMono<XRCustomRaycastController>
{
    private InputDevice rightController;
    private InputDevice leftController;

    private XRRayInteractor rightRay;

    private RaycastHit hitInfo;

    private bool rightTriggrtDown = false;
  //  private int rightTriggerTimes = 0;
    private bool rightGripButtonDown = false;
    private bool leftGripButtonDown = false;

    private Vector3 rightDevicePosition;
    private Vector3 leftDevicePosition;

    private bool isRotatingModel = false;
    private bool isScalingModel = false;

    private bool isLeftInScaleMode = false;
    private bool isRightInScaleMode = false;

    private Quaternion rightDeviceRotation;

    private int rayState = 0;
    private Button3DCustom currentBtn;

    // Start is called before the first frame update
    void Start()
    {
        InitDevices();
        //XRSceneManager.GetInstance.cs += ChangeSceneInit;
        // Invoke("TestTele", 5f);
    }

    void TestTele()
    {
        // XRSceneManager.GetInstance.TeleportSameScene(GameObject.Find("portal1").GetComponent<ScenePortal>()
        //     .toPortal);

        XRSceneManager.GetInstance.TeleportDifferentScene("SceneA");
    }

    void ChangeSceneInit()
    {
        InitDevices();
        //Debug.Log("ChangeSceneInit");
    }

    //初始化设备信息
    //TODO:手柄掉綫重連？验证API 
    void InitDevices()
    {
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        InputDevices.deviceConnected += RegisterDevices;

        rightRay = XROriginInstance.XROriginObj.transform.Find("Camera Offset/RightHand Controller")
            .GetComponent<XRRayInteractor>();
    }

    void RegisterDevices(InputDevice connectDevice)
    {
        if (connectDevice.isValid)
        {
            if ((connectDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
                leftController = connectDevice;
            }

            if ((connectDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                rightController = connectDevice;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 v;
        // rightController.TryGetFeatureValue(CommonUsages.devicePosition, out v);
        // XRDebug.Log(v.ToString()); 
        RayCastUpdate();

        //TODO:Update检测恢复
        // if (!rightController.isValid)
        // {
        //     
        // }
    }


    /// <summary>
    /// 右手炳射线检测，可自定义目标
    /// </summary>
    void RayCastUpdate()
    {
        if (rightRay.TryGetCurrent3DRaycastHit(out hitInfo) && hitInfo.collider != null)
        {
            // XRDebug.Log(hitInfo.point.ToString());
            // TODO：传送检测封装，输入系统封装
            if (hitInfo.collider.gameObject.GetComponent<ScenePortal>())
            {
                if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggrtDown) &&
                    rightTriggrtDown)
                {
                    XRDebug.Log("hit:" + hitInfo.collider.gameObject.name);
                    //TODO 测试，后续自动化添加tag
                    if (hitInfo.collider.gameObject.GetComponent<ScenePortal>().isSameScene)
                    {
                        XRSceneManager.GetInstance.TeleportSameScene(hitInfo.collider.gameObject
                            .GetComponent<ScenePortal>()
                            .toPortal);
                    }
                    else
                    {
                        XRDebug.Log(hitInfo.collider.gameObject
                            .GetComponent<ScenePortal>().DifferentSceneName);
                        XRSceneManager.GetInstance.TeleportDifferentScene(hitInfo.collider.gameObject
                            .GetComponent<ScenePortal>().DifferentSceneName);
                    }
                }
            }

            //:沙盘旋转
            if (hitInfo.collider.CompareTag("sandBoxModel"))
            {
                if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripButtonDown))
                {
                    if (rightGripButtonDown)
                    {
                        isRotatingModel = true;
                        if (!isScalingModel)
                        {
                            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightDevicePosition);
                            rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out rightDeviceRotation);
                            SandBoxModule.GetInstance().DoRotateModel(rightDevicePosition, rightDeviceRotation);
                        }
                    }
                    else
                    {
                        if (isRotatingModel)
                        {
                            SandBoxModule.GetInstance().StopRotateModel();
                            isRotatingModel = false;
                        }
                    }
                }

                //射線指向退出沙盤
                // if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggrtDown))
                // {
                //     if (rightTriggrtDown)
                //     {
                //         rightTriggerTimes++;
                //         if (rightTriggerTimes == 1)
                //         {
                //             EventDispatcher.GetInstance().DispatchEvent("change_sandBoxMode");
                //         }
                //     }
                //     else
                //     {
                //         if (rightTriggerTimes != 0)
                //         {
                //             rightTriggerTimes = 0;
                //         }
                //     }
                // }
            }

            //TODO：后续统一封装，3DButton触发以及onenter、highlight、onexit等触发
            if (hitInfo.collider.CompareTag("3DButton"))
            {
                if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightTriggrtDown))
                {
                    if (rightTriggrtDown)
                    {
                        XRHandleData.rightTriggerTimes++;
                        if (XRHandleData.rightTriggerTimes == 1)
                        {
                            hitInfo.collider.GetComponent<Button3DCustom>().OnClick();
                        }
                    }
                    else
                    {
                        if (XRHandleData.rightTriggerTimes != 0)
                        {
                            XRHandleData.rightTriggerTimes = 0;
                        }
                    }
                }

                if (rayState != 1)
                {
                    rayState = 1;
                    currentBtn = hitInfo.collider.GetComponent<Button3DCustom>();
                    currentBtn.OnRayEnter();
                }
            }
        }
        else
        {
            if (rayState != 0)
            {
                rayState = 0;
                currentBtn.OnRayExit();
                currentBtn = null;
            }
        }


        //沙盘缩放
        if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripButtonDown))
        {
            if (rightGripButtonDown)
            {
                isRightInScaleMode = true;
            }
            else
            {
                isRightInScaleMode = false;
            }
        }

        if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripButtonDown))
        {
            if (leftGripButtonDown)
            {
                isLeftInScaleMode = true;
            }
            else
            {
                isLeftInScaleMode = false;
            }
        }

        if (isRightInScaleMode && isLeftInScaleMode)
        {
            isScalingModel = true;

            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out rightDevicePosition);
            leftController.TryGetFeatureValue(CommonUsages.devicePosition, out leftDevicePosition);

            SandBoxModule.GetInstance().DoScaleModel(leftDevicePosition, rightDevicePosition);

            //TODO：添加bool判断，缩放时禁用旋转
            if (rightRay.enabled)
            {
                rightRay.enabled = false;
            }
        }
        else
        {
            if (isScalingModel)
            {
                SandBoxModule.GetInstance().StopScaleModel();
                isScalingModel = false;
                rightRay.enabled = true;
            }
        }
    }
}
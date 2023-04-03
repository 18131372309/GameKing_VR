using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class XRCustomRaycastController : SingleTonMono<XRCustomRaycastController>
{
    private InputDevice rightController;

    private XRRayInteractor rightRay;

    private RaycastHit hitInfo;

    private bool rightTriggrtDown = false;

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
    //TODO:手柄掉綫重連 
    void InitDevices()
    {
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        rightRay = XROriginInstance.XROriginObj.transform.Find("Camera Offset/RightHand Controller")
            .GetComponent<XRRayInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        RayCastUpdate();
    }

    /// <summary>
    /// 右手炳射线检测，可自定义目标
    /// </summary>
    void RayCastUpdate()
    {
        if (rightRay.TryGetCurrent3DRaycastHit(out hitInfo) && hitInfo.collider != null)
        {
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
        }
    }
}
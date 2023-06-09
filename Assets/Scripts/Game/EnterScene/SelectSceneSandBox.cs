using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectSceneSandBox : MonoBehaviour
{
    private InputDevice rightController;

    private XRRayInteractor rightRay;

    private RaycastHit hitInfo;

    private bool triggerButtonDown = false;
    // private int rightTriggerTimes = 0;

    public Button3DCustom[] enterSceneBtn;
    private Button3DCustom currentBtn;

    private int rayState = 0;

    // private void Awake()
    // {
    //    EventDispatcher.GetInstance().Register("handleModel",ShowGuide);
    // }

    // Start is called before the first frame update
    void Start()
    {
        InitDevices();
        InitListener();
    }

    void InitListener()
    {
        foreach (var btn in enterSceneBtn)
        {
            btn.AddListener(() =>
            {
                SceneManager.LoadScene(btn.gameObject.transform.parent.name);
                Debug.Log("ID:" + btn.GetInstanceID());
            });
        }
    }


    void InitDevices()
    {
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevices.deviceConnected += RegisterDevices;
        rightRay = XROriginInstance.XROriginObj.transform.Find("Camera Offset/RightHand Controller")
            .GetComponent<XRRayInteractor>();

        HideHandle();
        //TODO:修改XR底层逻辑，添加手柄生成后的事件监听
        // Invoke("ShowGuide",2f);
        StartCoroutine(ShowGuide());
    }

    void HideHandle()
    {
       // XRUtils.GetInstance().SetHandleModelState(false);
    }

    IEnumerator ShowGuide()
    {
        yield return new WaitForSeconds(0.5f);
        XRPlayerGuide.GetInstance().ChangeHandleGuide(GuideMode.EnterSceneGuide);
    }

    void RegisterDevices(InputDevice connectDevice)
    {
        if (connectDevice.isValid)
        {
            if ((connectDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                rightController = connectDevice;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     foreach (var btn in enterSceneBtn)
        //     {
        //         btn.OnClick();
        //     }
        // }

        RayCheckForSelectScene();
    }

    void RayCheckForSelectScene()
    {
        if (rightRay.TryGetCurrent3DRaycastHit(out hitInfo) && hitInfo.collider != null)
        {
            //场景切换
            // if (hitInfo.collider.CompareTag("sandBoxModel"))
            // {
            //     if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonDown) &&
            //         triggerButtonDown)
            //     {
            //         SceneManager.LoadScene(hitInfo.collider.name);
            //     }
            // }

            //3D按钮显示
            if (hitInfo.collider.CompareTag("3DButton"))
            {
                if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonDown))
                {
                    if (triggerButtonDown)
                    {
                        //TODO:全局管理times，防止切场景时被重置,抬起监听
                        XRHandleData.rightTriggerTimes++;
                        if (XRHandleData.rightTriggerTimes == 1)
                        {
                            // XRDebug.Log("3DButton");
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
    }

    private void OnDestroy()
    {
    }
}
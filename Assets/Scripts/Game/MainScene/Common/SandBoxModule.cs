using System.Collections;
using System.Collections.Generic;
using Unity.XR.Qiyu;
using UnityEngine;

public class SandBoxModule : SingleTonClass<SandBoxModule>
{
    private Vector3 originPos;

    private Transform sanboxView;

    private GameObject sandBoxModel;

    private GameObject landModel;

    private Vector3 speedV3 = Vector3.zero;

    private Vector3 lastDevicePosition;
    private Quaternion lastDeviceRotation;

    private float lastDistance = 0;
    private float currentDistance = 0;

    private int DoFrameTimeR = 0;
    private int DoFrameTimeS = 0;

    private float baseSpeed = 50f;
    private float axisPosXRate = 0.5f;
    private float axisRotationYRate = 1f;

    private float axisX = 0;
    private float axisRotationY = 0;

    private float axisDistance = 0;

    private int direction = -1;

    private bool isSandboxMode = false;

    private float scaleDefault = 0.5f;
    private float scaleLimitUp = 3.5f;
    private float scaleLimitDown = 0.3f;

    private Vector3 originSandBoxScale;

    private Button3DCustom enterBtn;
    private Button3DCustom returnBtn;

    /// <summary>
    /// 數據初始化
    /// </summary>
    public void InitData()
    {
        originPos = XROriginInstance.XROriginObj.transform.position;
        sanboxView = GameObject.Find("SandBoxViewPoint").transform;
        // XROriginInstance.curViewMode = ViewMode.Ground;
        //EventDispatcher.GetInstance().Register("change_sandBoxMode", ChangeSandBoxMode);

        sandBoxModel = GameObject.Find("SandBoxModel");
        landModel = GameObject.Find("LandModel");

        enterBtn = GameObject.Find("enter3DButton_SD").GetComponent<Button3DCustom>();
        returnBtn = GameObject.Find("return3DButton_SD").GetComponent<Button3DCustom>();

        lastDevicePosition = Vector3.zero;
        lastDeviceRotation = Quaternion.identity;
        lastDistance = 0;

        sandBoxModel.transform.localScale = sandBoxModel.transform.localScale * (1 + scaleDefault);
        originSandBoxScale = sandBoxModel.transform.localScale;


        InitListener();
    }

    void InitListener()
    {
        enterBtn.AddListener(() =>
        {
            UIManager.GetInstance.ChangeViewMode(ViewMode.Ground);
            UIManager.GetInstance.SetViewModeIcon(ViewMode.Ground);
        });

        returnBtn.AddListener(() => { UIManager.GetInstance.ReturnEnterScene(); });
    }


    public void OnEnter()
    {
        if (!isSandboxMode)
        {
            //先进行状态切换
            FirstViewModule.GetInstance().OnExit();
            GodViewModlue.GetInstance().OnExit();
            UIManager.GetInstance.ShowViewModeSelectUi(2);

            landModel.SetActive(false);
            sandBoxModel.SetActive(true);
            XRUtils.GetInstance().SetPassThrough(true);
            //XRUtils.GetInstance().SetHandleModelState(false);

            XROriginInstance.curViewMode = ViewMode.SandBox;
            XRMovementController.GetInstance.TeleportXROrigin(sanboxView.position, Quaternion.identity);

            isSandboxMode = true;
        }
    }

    public void OnExit()
    {
        if (isSandboxMode)
        {
            landModel.SetActive(true);
            sandBoxModel.SetActive(false);
            XRUtils.GetInstance().SetPassThrough(false);
            //XRUtils.GetInstance().SetHandleModelState(true);

            isSandboxMode = false;
        }
    }


    /// <summary>
    /// 切换沙箱视角
    /// </summary>
    /// <param name="objs"></param>
    void ChangeSandBoxMode(object[] objs)
    {
        /*
        if (XROriginInstance.curViewMode == ViewMode.Ground || XROriginInstance.curViewMode == ViewMode.Sky)
        {
            landModel.SetActive(false);
            sandBoxModel.SetActive(true);
            SetPassThrough(true);

            XROriginInstance.curViewMode = ViewMode.SandBox;
            XRMovementController.GetInstance.TeleportXROrigin(sanboxView.position, Quaternion.identity);
        }
        else if (XROriginInstance.curViewMode == ViewMode.SandBox)
        {
            landModel.SetActive(true);
            sandBoxModel.SetActive(false);
            SetPassThrough(false);

            XRMovementController.GetInstance.moveLimt_y = originPos.y;
            XROriginInstance.curViewMode = ViewMode.Ground;
            XRMovementController.GetInstance.TeleportXROrigin(originPos, Quaternion.identity);
        }
        */
        // if ((ViewMode) objs[0] == ViewMode.SandBox)
        // {
        //     _sandBoxOnEnter.Invoke();
        //   
        // }
    }

    // protected override void UnInit()
    // {
    //     base.UnInit();
    //     EventDispatcher.GetInstance().UnRegister("change_sandBoxMode", ChangeSandBoxMode);
    // }

    /// <summary>
    /// 沙盤旋轉
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="rotation"></param>
    public void DoRotateModel(Vector3 positon, Quaternion rotation)
    {
        DoFrameTimeR++;
        if (DoFrameTimeR <= 1)
        {
            lastDevicePosition = positon;
            lastDeviceRotation = rotation;
        }
        else
        {
            axisX = positon.x - lastDevicePosition.x;
            axisRotationY = rotation.eulerAngles.y - lastDeviceRotation.eulerAngles.y;

            speedV3 = Vector3.up * (axisX * axisPosXRate + axisRotationY * axisRotationYRate) * direction;
            sandBoxModel.transform.Rotate(speedV3);

            lastDevicePosition = positon;
            lastDeviceRotation = rotation;
        }
    }

    /// <summary>
    /// 沙盘缩放
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void DoScaleModel(Vector3 left, Vector3 right)
    {
        currentDistance = Vector3.Distance(left, right);

        DoFrameTimeS++;

        if (DoFrameTimeS <= 1)
        {
            lastDistance = currentDistance;
        }

        axisDistance = currentDistance - lastDistance;
        // XRDebug.Log("axisDistance:" + axisDistance.ToString());

        sandBoxModel.transform.localScale = sandBoxModel.transform.localScale * (1 + axisDistance);

        // 范围限制
        if (sandBoxModel.transform.localScale.IsGreaterThan(originSandBoxScale * (1 + scaleLimitUp)))
        {
            sandBoxModel.transform.localScale = originSandBoxScale * (1 + scaleLimitUp);
        }
        else if (sandBoxModel.transform.localScale.IsLessThan(originSandBoxScale * (1 - scaleLimitDown)))
        {
            sandBoxModel.transform.localScale = originSandBoxScale * (1 - scaleLimitDown);
        }

        lastDistance = currentDistance;
    }


    public void StopRotateModel()
    {
        DoFrameTimeR = 0;
    }

    public void StopScaleModel()
    {
        DoFrameTimeS = 0;
    }
}
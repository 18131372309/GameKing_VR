using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRTeleportController : SingleTonMono<XRTeleportController>
{
    private GameObject rightControllerObj;

    //  private GameObject rightControllerObjT;
    private GameObject[] teleportAreas;
    private bool isEnter = false;
    private bool isExit = false;

    protected override void Init()
    {
        base.Init();
    }

    private void Start()
    {
        rightControllerObj = XROriginInstance.XROriginObj.transform.Find("Camera Offset/RightHand Controller")
            .gameObject;
        // rightControllerObjT = XROriginInstance.XROriginObj.transform.Find("Camera Offset/RightHand ControllerT").gameObject;
        teleportAreas = GameObject.FindGameObjectsWithTag("teleportArea");
    }

    public void OnEnterTeleport()
    {
        if (isEnter)
        {
            return;
        }

        isExit = false;
        isEnter = true;

        // XRDebug.Log("OnEnterTeleport");

        OnTeleportState();
    }

    public void OnStayTeleport()
    {
    }

    public void OnExitTeleport()
    {
        if (isExit)
        {
            return;
        }

        isExit = true;
        isEnter = false;

        //XRDebug.Log("OnExitTeleport");

        OnInteractionState();
    }

    void OnTeleportState()
    {
        // rightControllerObjT.SetActive(true);
        // rightControllerObj.SetActive(false);


        SetTeleportArea(true);
        rightControllerObj.GetComponent<XRController>().selectUsage = InputHelpers.Button.PrimaryAxis2DUp;
        rightControllerObj.GetComponent<XRRayInteractor>().lineType = XRRayInteractor.LineType.ProjectileCurve;
    }

    void OnInteractionState()
    {
        // rightControllerObjT.SetActive(false);
        // rightControllerObj.SetActive(true);
        //StartCoroutine(ExitTel());
        SetTeleportArea(false);
        rightControllerObj.GetComponent<XRController>().selectUsage = InputHelpers.Button.TriggerButton;
        rightControllerObj.GetComponent<XRRayInteractor>().lineType = XRRayInteractor.LineType.StraightLine;
    }

    void SetTeleportArea(bool state)
    {
        foreach (var area in teleportAreas)
        {
            area.GetComponent<TeleportationArea>().enabled = state;
        }
    }

    private IEnumerator ExitTel()
    {
        yield return new WaitForEndOfFrame();
        rightControllerObj.GetComponent<XRController>().selectUsage = InputHelpers.Button.TriggerButton;
        rightControllerObj.GetComponent<XRRayInteractor>().lineType = XRRayInteractor.LineType.StraightLine;
    }

    protected override void UnInit()
    {
        base.UnInit();
    }
}
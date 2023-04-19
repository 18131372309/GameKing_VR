using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstViewModule : SingleTonClass<FirstViewModule>
{

    private GameObject sandBoxModel;

    private GameObject landModel;
    private float groundLimit_Y = 0f; 
    
    private bool isFirstViewMode = false;
    
    public void InitData()
    {
        sandBoxModel = GameObject.Find("SandBoxModel");
        landModel = GameObject.Find("LandModel");
        groundLimit_Y = XROriginInstance.XROriginObj.transform.position.y;
        
        XRMovementController.GetInstance.tempXRPosition = XROriginInstance.XROriginObj.transform.position;
    }
    
    public void OnEnter()
    {
        if (!isFirstViewMode)
        {
            GodViewModlue.GetInstance().OnExit();
            SandBoxModule.GetInstance().OnExit();
            UIManager.GetInstance.ShowViewModeSelectUi(2);
            
            landModel.SetActive(true);
            sandBoxModel.SetActive(false);
            XRUtils.GetInstance().SetPassThrough(false);
            
            XRMovementController.GetInstance.moveLimt_y = groundLimit_Y;
            XROriginInstance.curViewMode = ViewMode.Ground;
            XRMovementController.GetInstance.TeleportXROrigin(XRMovementController.GetInstance.tempXRPosition, Quaternion.identity);
            
            isFirstViewMode = true;
           
        }
        
    }

    public void OnExit()
    {
        if (isFirstViewMode)
        {
            Debug.Log("tempXRPosition-update");
            XRMovementController.GetInstance.tempXRPosition = XROriginInstance.XROriginObj.transform.position;
            isFirstViewMode = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodViewModlue : SingleTonClass<GodViewModlue>
{
    private GameObject sandBoxModel;

    private GameObject landModel;
    
    private float skyLimit_Y = 80;
    
    private bool isGodViewMode = false;
    
    public void InitData()
    {
        sandBoxModel = GameObject.Find("SandBoxModel");
        landModel = GameObject.Find("LandModel");
    }
    
    public void OnEnter()
    {
        if (!isGodViewMode)
        {
            FirstViewModule.GetInstance().OnExit();
            SandBoxModule.GetInstance().OnExit();
            UIManager.GetInstance.ShowViewModeSelectUi(2);
            
            landModel.SetActive(true);
            sandBoxModel.SetActive(false);
            XRUtils.GetInstance().SetPassThrough(false);
            
         //   XRMovementController.GetInstance.tempXRPosition = XROriginInstance.XROriginObj.transform.position;
            XRMovementController.GetInstance.moveLimt_y = skyLimit_Y;
            XROriginInstance.curViewMode = ViewMode.Sky;
            XRMovementController.GetInstance.TeleportXROrigin(new Vector3(0, skyLimit_Y, 0), Quaternion.identity);

            isGodViewMode = true;
          
        }
        
    }

    public void OnExit()
    {
        if (isGodViewMode)
        {
            isGodViewMode = false;
        }
    }
}

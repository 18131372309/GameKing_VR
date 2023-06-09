using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public enum GuideMode
{
    FirstViewGuide,
    GodViewGuide,
    SandBoxGuide,
    EnterSceneGuide
}

public class XRPlayerGuide : SingleTonClass<XRPlayerGuide>
{
    private GameObject leftHandle;
    private GameObject rightHandle;

    private GameObject guide_teleport;
    private GameObject guide_menu;
    private GameObject guide_confirm;
    private GameObject guide_rotate;


    private GameObject guide_scale;
    private GameObject guide_move;
    

    protected override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 切换引导指南
    /// </summary>
    /// <param name="guideMode">当前模式-GuideMode</param>
    public void ChangeHandleGuide(GuideMode guideMode)
    {
        InitData();
        switch (guideMode)
        {
            case GuideMode.FirstViewGuide:
                FirstViewGuide();
                break;
            case GuideMode.GodViewGuide:
                GodViewGuide();
                break;
            case GuideMode.SandBoxGuide:
                SandBoxGuide();
                break;
            case GuideMode.EnterSceneGuide:
                EnterSceneGuide();
                break;
            default:
                ResetGuide();
                break;
        }
    }
    

    void InitData()
    {
        if (leftHandle == null || rightHandle == null)
        {
            leftHandle = XROriginInstance.leftControllerObj.GetComponent<XRController>().model.gameObject;
            rightHandle = XROriginInstance.rightControllerObj.GetComponent<XRController>().model.gameObject;

            guide_teleport = rightHandle.transform.Find("Handle/BtnJoyStick/BtnJoyStick/guide").gameObject;
            guide_menu = rightHandle.transform.Find("Handle/BtnA/BtnA/guide").gameObject;
            guide_confirm = rightHandle.transform.Find("Handle/BtnTrigger/BtnTrigger/guide").gameObject;
            guide_rotate = rightHandle.transform.Find("Handle/BtnGrip/BtnGrip/guide").gameObject;

            guide_scale = leftHandle.transform.Find("Handle/BtnGrip/BtnGrip/guide").gameObject;
            guide_move = leftHandle.transform.Find("Handle/BtnJoyStick/BtnJoyStick/guide").gameObject;

            ResetGuide();
        }
    }

    // IEnumerator WaitModel()
    // {
    //     yield return new WaitUntil (() =>
    //     {
    //         return XROriginInstance.leftControllerObj.GetComponent<XRController>().model != null;
    //
    //     });
    // }

    void ResetGuide()
    {
        guide_teleport.SetActive(false);
        guide_menu.SetActive(false);
        guide_confirm.SetActive(false);
        guide_rotate.SetActive(false);

        guide_scale.SetActive(false);
        guide_move.SetActive(false);
    }

    void FirstViewGuide()
    {
        ResetGuide();
        guide_teleport.SetActive(true);
        guide_menu.SetActive(true);
        guide_confirm.SetActive(true);
        guide_rotate.SetActive(false);

        guide_scale.SetActive(false);
        guide_move.SetActive(true);
        
      //  SetHandleMeshRenderer(true);
    }

    void GodViewGuide()
    {
        ResetGuide();
        guide_teleport.SetActive(false);
        guide_menu.SetActive(true);
        guide_confirm.SetActive(true);
        guide_rotate.SetActive(false);

        guide_scale.SetActive(false);
        guide_move.SetActive(true);
        
       // SetHandleMeshRenderer(true);
    }
    //TODO:MR模式是否显示手柄
    void SandBoxGuide()
    {
        ResetGuide();
        guide_teleport.SetActive(false);
        guide_menu.SetActive(false);
        guide_confirm.SetActive(true);
        guide_rotate.SetActive(true);

        guide_scale.SetActive(true);
        guide_move.SetActive(false);
        
      //  SetHandleMeshRenderer(false);
    }

    //TODO:MR模式是否显示手柄
    void EnterSceneGuide()
    {
        ResetGuide();
        guide_teleport.SetActive(false);
        guide_menu.SetActive(false);
        guide_confirm.SetActive(true);
        guide_rotate.SetActive(false);

        guide_scale.SetActive(false);
        guide_move.SetActive(false);
        
      //  SetHandleMeshRenderer(false);
    }

    //单独设置手柄样式MeshRenderer可见性
    void SetHandleMeshRenderer(bool visible)
    {
        DoSetMeshRender(rightHandle.transform, visible);
        DoSetMeshRender(leftHandle.transform, visible);
    }

    //TODO：缓存数据，对比查找
    void DoSetMeshRender(Transform root, bool visiable)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).GetComponent<MeshRenderer>() != null)
            {
                root.GetChild(i).GetComponent<MeshRenderer>().enabled = visiable;
            }

            DoSetMeshRender(root.GetChild(i), visiable);
        }
    }
}
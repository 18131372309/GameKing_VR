using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewModeSelectUI : UIBase, IUITypeSet
{
    private GameObject FirstView;
    private GameObject GodView;
    private GameObject SandBoxView;

    public Sprite[] firstViewSprites;
    public Sprite[] godViewSprites;
    public Sprite[] sandBoxViewSprites;

    private void InitData()
    {
        FirstView = FindChildByName("FirstViewButton");
        GodView = FindChildByName("GodViewButton");
        SandBoxView = FindChildByName("SandBoxViewButton");
        InitListener();

        //TODO:全局时间线
    }

    void InitListener()
    {
        FirstView.GetComponent<Button>().onClick.AddListener(FirstViewMode);
        GodView.GetComponent<Button>().onClick.AddListener(GodViewMode);
        SandBoxView.GetComponent<Button>().onClick.AddListener(SandBoxViewMode);
        FindChildByName("ReturnEnterSceneButton").GetComponent<Button>().onClick.AddListener(ReturnEnterScene);
    }

    public void InitDefaultViewMode()
    {
        InitData();
        SandBoxViewMode();
       
    }

   

    public void SetThisUIType()
    {
        uiType = UITypeEnum.XRFollowCanvas;
    }

    public override void ShowUI()
    {
        base.ShowUI();
        //TODO worldSpace模式的UI每次展示需要动态修改坐标
        //  this.gameObject.transform.SetParent(GetCanvasByUIType(uiType).transform);
    }

    public override void HideUI()
    {
        base.HideUI();
    }

    //选中状态,贴图样式，透明度，字体加粗
    void FirstViewMode()
    {
        SetFirstViewIcon();
        UIManager.GetInstance.ChangeViewMode(ViewMode.Ground);
    }

    public void SetFirstViewIcon()
    {
        ResetViewSprite();
        FirstView.GetComponent<Image>().sprite = firstViewSprites[1];
        FirstView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
        FirstView.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
    }

    void GodViewMode()
    {
        SetGodViewIcon();
        UIManager.GetInstance.ChangeViewMode(ViewMode.Sky);
    }

    public void SetGodViewIcon()
    {
        ResetViewSprite();
        GodView.GetComponent<Image>().sprite = godViewSprites[1];
        GodView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
        GodView.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
    }

    void SandBoxViewMode()
    {
        SetBoxViewIcon();

        UIManager.GetInstance.ChangeViewMode(ViewMode.SandBox);
    }

    public void SetBoxViewIcon()
    {
        ResetViewSprite();
        SandBoxView.GetComponent<Image>().sprite = sandBoxViewSprites[1];
        SandBoxView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
        SandBoxView.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
    }

    /// <summary>
    /// 重置效果
    /// </summary>
    void ResetViewSprite()
    {
        FirstView.GetComponent<Image>().sprite = firstViewSprites[0];
        GodView.GetComponent<Image>().sprite = godViewSprites[0];
        SandBoxView.GetComponent<Image>().sprite = sandBoxViewSprites[0];

        FirstView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);
        GodView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);
        SandBoxView.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);

        FirstView.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        GodView.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
        SandBoxView.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
    }

    void ReturnEnterScene()
    {
        UIManager.GetInstance.ReturnEnterScene();
    }
}
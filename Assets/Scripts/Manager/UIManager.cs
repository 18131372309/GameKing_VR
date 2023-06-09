using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : SingleTonMono<UIManager>
{
    private ViewModeSelectUI _viewModeSelectUi;

    private string UiPath;
    // private GameObject xrFollowCanvas;
    // private GameObject xrWorldCanvas;

    protected override void Init()
    {
        base.Init();
        UiPath = "MainScenePrefabs/UI/";
        _viewModeSelectUi = FindObjectOfType<ViewModeSelectUI>();
    }

    /// <summary>
    /// 显示视角选择的UI
    /// </summary>
    /// <param name="state">0默认，1显示，2隐藏</param>
    public void ShowViewModeSelectUi(int state = 0)
    {
        if (state == 0)
        {
            if (!_viewModeSelectUi.gameObject.activeInHierarchy)
            {
                _viewModeSelectUi.ShowUI();
            }
            else
            {
                _viewModeSelectUi.HideUI();
            }
        }
        else if (state == 1)
        {
            _viewModeSelectUi.ShowUI();
        }
        else if (state == 2)
        {
            _viewModeSelectUi.HideUI();
        }
    }

    public void InitDefaultViewMode()
    {
        _viewModeSelectUi.InitDefaultViewMode();
    }

    public void ChangeViewMode(ViewMode viewMode)
    {
        switch (viewMode)
        {
            case ViewMode.Ground:
                FirstViewModule.GetInstance().OnEnter();
                StartCoroutine(ShowGuide(GuideMode.FirstViewGuide));
                break;
            case ViewMode.Sky:
                GodViewModlue.GetInstance().OnEnter();
                StartCoroutine(ShowGuide(GuideMode.GodViewGuide));
                break;
            case ViewMode.SandBox:
                SandBoxModule.GetInstance().OnEnter();
                StartCoroutine(ShowGuide(GuideMode.SandBoxGuide));
                break;
        }
        
    }

    IEnumerator ShowGuide(GuideMode guideMode)
    {
        yield return new WaitForSeconds(0.5f);
        XRPlayerGuide.GetInstance().ChangeHandleGuide(guideMode);
    }
    

    public void SetViewModeIcon(ViewMode viewMode)
    {
        switch (viewMode)
        {
            case ViewMode.Ground:
                _viewModeSelectUi.SetFirstViewIcon();
                break;
            case ViewMode.Sky:
                _viewModeSelectUi.SetGodViewIcon();
                break;
            case ViewMode.SandBox:
                _viewModeSelectUi.SetBoxViewIcon();
                break;
        }
    }

    public void ReturnEnterScene()
    {
        FirstViewModule.GetInstance().OnExit();
        GodViewModlue.GetInstance().OnExit();
        SandBoxModule.GetInstance().OnExit();

        XRSceneManager.GetInstance.TeleportDifferentScene("EnterScene");
    }

    /*TODO:后续通过配置表提前加载好UI
    public void ShowXrUiWithName(TargetCanvasEnum targetCanvas, string UiName, bool state)
    {
        GameObject ui = GameObject.Find(UiName);
        GameObject targetCanvasObj = null;

        if (targetCanvas == TargetCanvasEnum.XRFollowCanvas)
        {
            targetCanvasObj = xrFollowCanvas;
        }
        else if (targetCanvas == TargetCanvasEnum.XRWorldCanvas)
        {
            targetCanvasObj = xrWorldCanvas;
        }
        else
        {
            targetCanvasObj = xrWorldCanvas;
        }

        if (ui == null)
        {
            ui = Instantiate((GameObject) Resources.Load(UiPath + UiName), Vector3.zero, Quaternion.identity,
                targetCanvasObj.transform);
        }
        ui.SetActive(state);
    }
    */
}
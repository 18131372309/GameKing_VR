using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XRLogManager : SingleTonMono<XRLogManager>
{
    private Button logButton;
    private Text logText;

    private GameObject logPanel;

    //  public Image img;
    // Start is called before the first frame update
    void Start()
    {
        Application.logMessageReceived += ReceiveLogMsg;
        InitData();
        //需要监听场景切换的，统一进行监听
        //  XRSceneManager.GetInstance.cs += ChangeSceneInit;
        // img.material.color=Color.cyan;
    }

    void ChangeSceneInit()
    {
        InitData();
        //Debug.Log("ChangeSceneInit");
    }

    void InitData()
    {
        Debug.Log("XRLogInit:");
        logPanel = XROriginInstance.XROriginObj.transform
            .Find("Camera Offset/Main Camera/XRFollowCanvas/MenuPanel/XRLog").gameObject;
        logButton = XROriginInstance.XROriginObj.transform
            .Find("Camera Offset/Main Camera/XRFollowCanvas/MenuPanel/XRLog/LogButton").GetComponent<Button>();
        logText = XROriginInstance.XROriginObj.transform
            .Find("Camera Offset/Main Camera/XRFollowCanvas/MenuPanel/XRLog/LogText").GetComponent<Text>();
        if (logButton != null)
        {
            logButton.onClick.AddListener(ShowLogText);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 设置LogPanel的显隐
    /// </summary>
    public void SetLogPanelStatus()
    {
        if (logPanel.activeInHierarchy)
        {
            logPanel.SetActive(false);
        }
        else
        {
            logPanel.SetActive(true);
        }
    }

    //显示日志
    void ShowLogText()
    {
        if (logText.gameObject.activeInHierarchy)
        {
            logText.gameObject.SetActive(false);
        }
        else
        {
            logText.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 日志监听打印
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="error"></param>
    /// <param name="lt"></param>
    void ReceiveLogMsg(string msg, string error, LogType lt)
    {
        if (logText != null)
        {
            if (lt == LogType.Exception)
            {
                //Debug.Log(error);
                logText.text = "<color='#FF0000'>" + msg + error + "</color>" + "\n" + logText.text;
            }

            if (lt == LogType.Log)
            {
                if (msg.StartsWith("XRLog"))
                {
                    logText.text = msg + "\n" + logText.text;
                }
            }
        }
    }

    //TODO 日志上传
}
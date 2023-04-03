using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ThisPortal
{
    Door1,
    Door2
}

public enum ToPortal
{
    Door1,
    Door2
}

//public delegate void ChangeScene();
public class XRSceneManager : SingleTonMono<XRSceneManager>
{
    private ScenePortal[] portals;

    private AsyncOperation asyncOperation;
    
  //  public ChangeScene cs;

    private void Start()
    {
        portals = GameObject.FindObjectsOfType<ScenePortal>();
    }

    //同场景传送
    public void TeleportSameScene(ToPortal toPortal)
    {

        foreach (var portal in portals)
        {
            
            if (portal.isSameScene && portal.thisPortal.ToString() == toPortal.ToString())
            {
               
                XRMovementController.GetInstance.TeleportXROrigin(portal.gameObject.transform.position,
                    portal.transform.rotation);
            }
        }
    }

    //跨场景传送
    public void TeleportDifferentScene(string sceneName)
    {
        //TODO：异步加载，进度条，过渡动画
        Debug.Log("LoadScene");
        // GameTimingManager.Instance.OnEnterScene(sceneName);
        SceneManager.LoadScene(sceneName);
      
        //cs.Invoke();
    }

    public void TeleportDiffenrtSceneAsync(string sceneName)
    {
        StartCoroutine(StartLoadSceneAsync(sceneName));
    }

    /// <summary>
    /// 异步跨场景传送
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator StartLoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        yield return asyncOperation;
        if (asyncOperation.isDone)
        {
            asyncOperation.allowSceneActivation = true;
            // GameTimingManager.Instance.OnEnterScene(sceneName);
        }
    }
}
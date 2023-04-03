using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public delegate void D_OnEnterScene(string name);

public class GameTimingManager : MonoBehaviour
{
    public static GameTimingManager Instance;
    private D_OnEnterScene _onEnterScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (Instance == null)
            {
                GameObject instanceObj = new GameObject("GameTimingManager");
                Instance = instanceObj.AddComponent<GameTimingManager>();
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }

    // public void AddListener(D_OnEnterScene listener)
    // {
    //     _onEnterScene += listener;
    // }

    // public void OnEnterScene(string name)
    // {
    //     _onEnterScene.Invoke(name);
    //     Debug.Log("_onEnterScene:"+name);
    // }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    /// <returns></returns>
    public string GetCurrentSceneName()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.name;
    }
}
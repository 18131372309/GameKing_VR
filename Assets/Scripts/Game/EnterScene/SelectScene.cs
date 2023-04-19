using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;


[Serializable]
public class ScenesConfig
{
    public SceneItemInfo[] Infos;
}

[Serializable]
public class SceneItemInfo
{
    public string sceneName;
    public string targetScene;
    public string description;
    public string image_url;
}

public class SelectScene : MonoBehaviour
{
    // public GameObject sceneItem1;
    // public GameObject sceneItem2;
    // public GameObject sceneItem3;

    private List<Vector3> itemPositionList;
    private List<GameObject> itemObjList;
    public Transform itemParent;
    private GameObject sceneItemPrefab;
    public GameObject lastBtbObj;
    public GameObject nextBtnObj;

    private int currentPageIndex = 0;

    private int startIndex = 0;

    private void Start()
    {
        //  InitListener();
        InitData();
        Debug.Log("ssss" + XRSettings.eyeTextureResolutionScale);
       // Invoke("ShowNextSceneItem",2f);
      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene("SceneSMYT");
        }
    }

    void InitData()
    {
        sceneItemPrefab = Resources.Load("EnterScenePrefabs/sceneItem") as GameObject;
        itemPositionList = new List<Vector3>();
        itemObjList = new List<GameObject>();

        lastBtbObj.GetComponent<Button>().onClick.AddListener(ShowLastSceneItem);
        nextBtnObj.GetComponent<Button>().onClick.AddListener(ShowNextSceneItem);

        lastBtbObj.SetActive(false);
        nextBtnObj.SetActive(false);

        LoadSceneConfig();
    }

    //动态布局
    void ShowLastSceneItem()
    {
        if (!nextBtnObj.activeInHierarchy)
        {
            nextBtnObj.SetActive(true);
        }

        currentPageIndex--;
        if (currentPageIndex == 0)
        {
            lastBtbObj.SetActive(false);
        }

        itemParent.DOLocalMove(itemPositionList[currentPageIndex], 0.1f);


        startIndex--;
        SetCurvedUI(startIndex);
    }

    void ShowNextSceneItem()
    {
        if (!lastBtbObj.activeInHierarchy)
        {
            lastBtbObj.SetActive(true);
        }

        currentPageIndex++;
        if (currentPageIndex == itemPositionList.Count - 3)
        {
            nextBtnObj.SetActive(false);
        }

        Debug.Log(itemPositionList[currentPageIndex]);
        itemParent.DOLocalMove(itemPositionList[currentPageIndex], 0.1f);

        startIndex++;
        SetCurvedUI(startIndex);
    }

    // 多个场景时（>3），位置排列
    /// <summary>
    /// 加载json场景配置并实例化
    /// </summary>
    void LoadSceneConfig()
    {
        string url = Application.streamingAssetsPath + "/ScenesConfig.json";
        NetWorkManager.GetInstance.GetStringOnline(url, (text) =>
        {
            ScenesConfig config = JsonUtility.FromJson<ScenesConfig>(text);
            // Debug.Log(config.Infos.Length);
            // Debug.Log(config.Infos[2].targetScene);
            for (int i = 0; i < config.Infos.Length; i++)
            {
                GameObject sceneItemClone = Instantiate(sceneItemPrefab);
                sceneItemClone.transform.SetParent(itemParent);


                Vector3 tempV3 = new Vector3(-80 + i * 80, 0, 0);
                Vector3 tempPV3 = new Vector3(-80 * i, 0, 0);
                itemPositionList.Add(tempPV3);
                sceneItemClone.transform.localPosition = tempV3;
                sceneItemClone.transform.localEulerAngles = new Vector3(0, 0, 0);
                sceneItemClone.name = config.Infos[i].sceneName;
                sceneItemClone.GetComponent<SelectItemEffect>().targetSceneName = config.Infos[i].targetScene;
                itemObjList.Add(sceneItemClone);
                string image_url = Application.streamingAssetsPath + "/" + config.Infos[i].image_url;

                //加载图片
                NetWorkManager.GetInstance.GetTextureOnline(image_url,
                    (texture) => { sceneItemClone.GetComponent<RawImage>().texture = texture; });
            }

            if (config.Infos.Length > 3)
            {
                nextBtnObj.SetActive(true);
            }

            startIndex = 0;
            SetCurvedUI(startIndex);
        });
    }

    void SetCurvedUI(int startIndex)
    {
        Debug.Log(itemObjList.Count);
        for (int i = 0; i < itemObjList.Count; i++)
        {
            itemObjList[i].transform.parent = itemParent;
        }

        int j = 0;
        //TODO: 待测试
        for (int i = startIndex; i < startIndex + 3; i++)
        {
            itemObjList[i].transform.parent = itemParent.parent.GetChild(j);
            itemObjList[i].transform.localEulerAngles = Vector3.zero;
            itemObjList[i].transform.localPosition = Vector3.zero;

            j++;
        }

        // int index = 0;
        // foreach (var VARIABLE in itemObjList)
        // {
        //     Debug.Log(VARIABLE.name);
        //     VARIABLE.transform.parent = itemParent.GetChild(index);
        //     index++;
        // }
    }

    /*
    /// <summary>
    /// 按键监听，目前固定的，之前有接口了再改成动态的
    /// </summary>
    void InitListener()
    {
        sceneItem1.GetComponent<Button>().onClick.AddListener(() =>
        {
            string sceneName = sceneItem1.GetComponent<SelectItemEffect>().targetSceneName;
            if (sceneName != "" && sceneName != String.Empty)
            {
                SceneManager.LoadScene(sceneName);
            }
        });
        sceneItem2.GetComponent<Button>().onClick.AddListener(() =>
        {
            string sceneName = sceneItem2.GetComponent<SelectItemEffect>().targetSceneName;

            if (sceneName != "" && sceneName != String.Empty)
            {
                SceneManager.LoadScene(sceneName);
            }
        });
        sceneItem3.GetComponent<Button>().onClick.AddListener(() =>
        {
            string sceneName = sceneItem3.GetComponent<SelectItemEffect>().targetSceneName;

            if (sceneName != "" && sceneName != String.Empty)
            {
                SceneManager.LoadScene(sceneName);
            }
        });
    }
    */
}
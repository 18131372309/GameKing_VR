using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UITypeEnum
{
    XRFollowCanvas,
    XRWorldCanvas
}

public class UIBase : MonoBehaviour
{
    protected UITypeEnum uiType;

    public virtual void ShowUI()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void HideUI()
    {
        this.gameObject.SetActive(false);    
    }


    public GameObject FindChildByName( string name)
    {
        return DoFindChild(this.gameObject.transform, name);
    }

    GameObject DoFindChild(Transform par, string name)
    {
        GameObject go = null;
        for (int i = 0; i <par.childCount; i++)
        {
            if (par.GetChild(i).name == name)
            {
                go = par.GetChild(i).gameObject;
            }

            DoFindChild(par.GetChild(i), name);
        }

        return go;
    }

    /// <summary>
    /// 根据UI类型。找到对应canvas
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns></returns>
    public GameObject GetCanvasByUIType(UITypeEnum uiType)
    {
        if (uiType == UITypeEnum.XRFollowCanvas)
        {
            return GameObject.Find("XRFollowCanvas");
        }
        else if (uiType == UITypeEnum.XRWorldCanvas)
        {
            return GameObject.Find("XRWorldCanvas");
        }

        return null;
    }
}
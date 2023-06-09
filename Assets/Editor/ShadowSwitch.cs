using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class ShadowSwitch
{
    [MenuItem("Window/Tools/ShadowTool/Shadow-ON")]
    public static void ShadowOn()
    {
        GameObject[] roots = Selection.gameObjects;
        foreach (var root in roots)
        {
            Debug.Log(root.name + "：隐藏开启");
            DoSearch(root.transform, true);
        }
    }

    [MenuItem("Window/Tools/ShadowTool/Shadow-OFF")]
    public static void ShadowOff()
    {
        GameObject[] roots = Selection.gameObjects;
        foreach (var root in roots)
        {
            Debug.Log(root.name + "：阴影关闭");
            DoSearch(root.transform, false);
        }
    }

    static void DoSearch(Transform root, bool state)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).GetComponent<MeshRenderer>())
            {
                if (state)
                {
                    root.GetChild(i).GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
                }
                else
                {
                    root.GetChild(i).GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                }
            }

            DoSearch(root.GetChild(i), state);
        }
    }
}
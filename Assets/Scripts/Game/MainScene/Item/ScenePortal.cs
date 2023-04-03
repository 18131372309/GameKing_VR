using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScenePortal : MonoBehaviour
{
    [Tooltip("是否是同场景传送")] public bool isSameScene;
    [Tooltip("跨场景传送的Scene名称")] public string DifferentSceneName = String.Empty;
    [Tooltip("当前传送点名称")] public ThisPortal thisPortal;
    [Tooltip("目标传送点名称")] public ToPortal toPortal;
}
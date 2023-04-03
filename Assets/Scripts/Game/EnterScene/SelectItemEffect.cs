using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectItemEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("传送的目标场景")] public string targetSceneName;

    private Vector3 originScale;
    private Vector3 targetScale;


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(targetScale, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originScale, 0.2f);
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeScene);
        originScale = transform.localScale;
        targetScale = originScale * 1.1f;
    }

    private void ChangeScene()
    {
        if (targetSceneName != null)
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
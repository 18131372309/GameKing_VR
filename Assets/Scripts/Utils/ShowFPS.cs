using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowFPS : SingleTonMono<ShowFPS>
{
    private Text FpsText;
    private float time;
    private int frameCount;
        
    void Awake()
    {
        FpsText = GetComponent<Text>();
       
    }

    /// <summary>
    /// 设置FPS 显隐
    /// </summary>
    void SetFpsPanel()
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }

    
    void Update()
    {
       UpdateFps();
    }

    /// <summary>
    /// 获取FPS值
    /// </summary>
    void UpdateFps()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            float fps = frameCount / time;
            time = 0;
            frameCount = 0;
            FpsText.text = fps.ToString("f2");
            FpsText.color = fps >= 20 ? Color.white : (fps > 15 ? Color.yellow : Color.red);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetWorkManager : SingleTonMono<NetWorkManager>
{
    private List<Coroutine> netCoroutines;

    private Coroutine getStringCo;
    
    private Coroutine getTextureCo;

    protected override void Init()
    {
        netCoroutines = new List<Coroutine>();
    }

    /// <summary>
    /// GET接口，获取string字符串
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void GetStringOnline(string url, UnityAction<string> callback)
    {
        getStringCo = StartCoroutine(IGetStringOnline(url, callback));
        if (!netCoroutines.Contains(getStringCo))
        {
            netCoroutines.Add(getStringCo);
        }
    }

    IEnumerator IGetStringOnline(string url, UnityAction<string> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            callback(webRequest.downloadHandler.text);
        }
    }
    
    /// <summary>
    /// GET接口，获取texture图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void GetTextureOnline(string url, UnityAction<Texture> callback)
    {
        getTextureCo = StartCoroutine(IGetTextureOnline(url, callback));
        if (!netCoroutines.Contains(getTextureCo))
        {
            netCoroutines.Add(getTextureCo);
        }
    }

    IEnumerator IGetTextureOnline(string url, UnityAction<Texture> callback)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
            callback(myTexture);
        }
    }

    protected override void UnInit()
    {
        foreach (var value in netCoroutines)
        {
            StopCoroutine(value);
        }

        netCoroutines.Clear();
    }
}
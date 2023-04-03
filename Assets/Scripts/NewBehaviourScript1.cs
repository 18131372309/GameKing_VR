using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Test",5f);
    }

    void Test()
    {
        XRSceneManager.GetInstance.TeleportDifferentScene("MainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewModeManager : SingleTonMono<ViewModeManager>
{
    // Start is called before the first frame update
    void Start()
    {
        SandBoxModule.GetInstance().InitData();
        FirstViewModule.GetInstance().InitData();
        GodViewModlue.GetInstance().InitData();
        
        UIManager.GetInstance.InitDefaultViewMode();
    }
}

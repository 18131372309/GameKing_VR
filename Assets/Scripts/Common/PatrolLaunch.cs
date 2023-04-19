using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLaunch : MonoBehaviour
{
    public GameObject target1;
    public GameObject par1;
    
    public GameObject target2;
    public GameObject par2;
    
    public GameObject target3;
    public GameObject par3;
    // Start is called before the first frame update
    void Start()
    {
        NpcPatrolController.GetInstance.StartNpcPatrol(target1,par1,6,false);
        NpcPatrolController.GetInstance.StartNpcPatrol(target2,par2,3);
        NpcPatrolController.GetInstance.StartNpcPatrol(target3,par3,5,false);

        NpcPatrolController.GetInstance.AddPatrolListener(target1, SS);
        NpcPatrolController.GetInstance.AddPatrolListener(target1, MM);
    }

    void SS(Transform trans)
    {
        Debug.Log(trans.name+"SS");
    }
    void MM(Transform trans)
    {
        Debug.Log(trans.name+"MM");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            NpcPatrolController.GetInstance.StopTargetNpcPatrol(target1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            NpcPatrolController.GetInstance.StopAllNpcPatrol();
        }
    }
}

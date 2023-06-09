using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcPatrolConfig
{
    [Header("NPC-当前Group名称（选填）")] public string NpcGroupName;
    [Header("NPC-预制体（可以是一组）")] public GameObject npcPrefab;
    [Header("NPC-巡逻点路径父节点")] public GameObject pointParent;
    [Header("NPC-巡逻点动画切换配置")] public PointState[] pointStates;
    [Header("NPC-移动速度")] [Range(1, 15)] public float speed;
    [Header("NPC-是否随机按照巡逻点移动")] public bool isRandom;
}

[Serializable]
public class PointState
{
    [Tooltip("动画状态机bool参数")] public string animatorSetBool;

    //TODO:添加到巡逻的yield return中，等待协程的时间
    [Tooltip("巡逻停止时间")] public float interruptTime = 0;
}

public class PatrolLaunch : MonoBehaviour
{
    [Header("是否绘制NPC路线Gizmos辅助")] public bool drawGizmos;

    [Space] [Header("NPC巡逻路径参数配置")] public NpcPatrolConfig[] NPC_Groups;


    // Start is called before the first frame update
    void Start()
    {
        InitPatrol();
    }

    void InitPatrol()
    {
        foreach (var npcPatrolConfig in NPC_Groups)
        {
            NpcPatrolController.GetInstance.StartNpcPatrol(npcPatrolConfig.npcPrefab, npcPatrolConfig.pointParent,
                npcPatrolConfig.speed, npcPatrolConfig.isRandom);
        }

        // NpcPatrolController.GetInstance.AddPatrolListener(target1, SS);
        // NpcPatrolController.GetInstance.AddPatrolListener(target1, MM);
    }

    void SS(Transform trans)
    {
        Debug.Log(trans.name + "SS");
    }

    void MM(Transform trans)
    {
        Debug.Log(trans.name + "MM");
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Transform pointPar = null;
            foreach (var npcPatrolConfig in NPC_Groups)
            {
                pointPar = npcPatrolConfig.pointParent.transform;
                for (int i = 0; i < pointPar.childCount; i++)
                {
                    if (pointPar.GetChild(i) != null)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(pointPar.GetChild(i).position, 0.7f);

                        int next = i + 1 < pointPar.childCount ? i + 1 : 0;

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(pointPar.GetChild(i).position,
                            pointPar.GetChild(next).position);
                    }
                }

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(npcPatrolConfig.npcPrefab.transform.position, 5f);

                Gizmos.color = Color.magenta;
                for (int i = 0; i < npcPatrolConfig.pointStates.Length; i++)
                {
                    if (npcPatrolConfig.pointStates[i].interruptTime != 0)
                    {
                        Gizmos.DrawSphere(pointPar.GetChild(i).position, 0.8f);
                    }
                }
            }
        }
    }
}
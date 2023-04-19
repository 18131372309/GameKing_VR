using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

//public delegate void PatrolBroadcast(GameObject go, Transform trans);
public class NpcPatrolController : MonoBehaviour
{
    private static NpcPatrolController _instance;

    private Dictionary<GameObject, Coroutine> _patrolCoroutineList;

    private Dictionary<GameObject, UnityAction<Transform>> _patrolBroadcastList;

    //  private PatrolBroadcast _patrolBroadcast;

    private static object _lock = new object();

    public static NpcPatrolController GetInstance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<NpcPatrolController>();

                    if (_instance == null)
                    {
                        GameObject singleTonMono = new GameObject();
                        _instance = singleTonMono.AddComponent<NpcPatrolController>();
                        singleTonMono.name = "(singleTonMono)" + typeof(NpcPatrolController).ToString();
                    }
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        InitData();
    }

    void InitData()
    {
        _patrolCoroutineList = new Dictionary<GameObject, Coroutine>();
        _patrolBroadcastList = new Dictionary<GameObject, UnityAction<Transform>>();
    }

    //-----------------------------------------------------巡逻功能-------------------------------------------------------

    /// <summary>
    /// Npc巡逻功能
    /// </summary>
    /// <param name="target">npc对象（GameObject）</param>
    /// <param name="pointsParent">巡逻点父节点（GameObject）</param>
    /// <param name="speed">速度（Float）</param>
    /// <param name="isRandom">是否随机点巡逻（bool默认随机）</param>
    public void StartNpcPatrol(GameObject target, GameObject pointsParent, float speed, bool isRandom = true)
    {
        if (target == null)
        {
            Debug.Log("<color=#FF6A6A>" + "请正确设置Npc实体!" + "</color>" + "<color=#FFFF00>" +
                      "-->检测接口调用：StartNpcPatrol" + "</color>");

            return;
        }

        int pointsLength = pointsParent.transform.childCount;

        if (pointsLength <= 1)
        {
            Debug.Log("<color=#FF6A6A>" + "请至少设置2个巡逻点位!" + "</color>" + "<color=#FFFF00>" +
                      "-->检测接口调用：StartNpcPatrol" + "</color>");

            return;
        }

        Transform[] points = new Transform[pointsLength];

        for (int i = 0; i < pointsLength; i++)
        {
            points[i] = pointsParent.transform.GetChild(i);
        }

        //是否随机巡逻
        if (!isRandom)
        {
            Coroutine cor = StartCoroutine(PatrolUpdateSequence(target, points, speed));
            if (!_patrolCoroutineList.ContainsKey(target))
            {
                _patrolCoroutineList.Add(target, cor);
            }
        }
        else
        {
            Coroutine cor = StartCoroutine(PatrolUpdateRandom(target, points, speed));
            if (!_patrolCoroutineList.ContainsKey(target))
            {
                _patrolCoroutineList.Add(target, cor);
            }
        }
    }

    /// <summary>
    /// 按巡逻点顺序 更新位置检测
    /// </summary>
    /// <param name="target"></param>
    /// <param name="points"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator PatrolUpdateSequence(GameObject target, Transform[] points, float speed)
    {
        int index = 0;
        float distance = 0;
        target.transform.LookAt(points[index].position);

        while (true)
        {
            // target.transform.Translate( Vector3.forward * speed * Time.deltaTime);
            target.transform.position = Vector3.MoveTowards(target.transform.position, points[index].position,
                speed * Time.deltaTime);

            distance = Vector3.Distance(target.transform.position, points[index].position);

            if (distance < 0.1f)
            {
                index++;
                if (index == points.Length)
                {
                    index = 0;
                }

                target.transform.LookAt(points[index].position);
                UpdatePatrolMessage(target, points[index]);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 随机巡逻点 更新位置检测
    /// </summary>
    /// <param name="target"></param>
    /// <param name="points"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator PatrolUpdateRandom(GameObject target, Transform[] points, float speed)
    {
        int index = Random.Range(0, points.Length);
        float distance = 0;
        target.transform.LookAt(points[index].position);

        while (true)
        {
            //target.transform.Translate( Vector3.forward * speed * Time.deltaTime);
            target.transform.position = Vector3.MoveTowards(target.transform.position, points[index].position,
                speed * Time.deltaTime);
            distance = Vector3.Distance(target.transform.position, points[index].position);

            if (distance < 0.1f)
            {
                index = Random.Range(0, points.Length);
                target.transform.LookAt(points[index]);
                UpdatePatrolMessage(target, points[index]);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 停止指定NPC巡逻
    /// </summary>
    /// <param name="target"></param>
    public void StopTargetNpcPatrol(GameObject target)
    {
        if (_patrolCoroutineList.ContainsKey(target))
        {
            StopCoroutine(_patrolCoroutineList[target]);
            _patrolCoroutineList.Remove(target);
            _patrolBroadcastList.Remove(target);
        }
    }

    /// <summary>
    /// 停止所有NPC巡逻
    /// </summary>
    public void StopAllNpcPatrol()
    {
        foreach (var VARIABLE in _patrolCoroutineList)
        {
            StopCoroutine(VARIABLE.Value);
        }

        _patrolCoroutineList.Clear();
        _patrolBroadcastList.Clear();
    }


    //---------------------------------------------------巡逻位置监听-----------------------------------------------------

    /// <summary>
    /// 添加巡逻npc位置监听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public void AddPatrolListener(GameObject target, UnityAction<Transform> handler)
    {
        if (target && handler != null)
        {
            if (!_patrolBroadcastList.ContainsKey(target))
            {
                _patrolBroadcastList.Add(target, handler);
            }
            else
            {
                _patrolBroadcastList[target] += handler;
            }
        }
    }

    /// <summary>
    /// 移除巡逻npc位置监听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public void RemovePatrolListener(GameObject target, UnityAction<Transform> handler)
    {
        if (_patrolBroadcastList.ContainsKey(target))
        {
            _patrolBroadcastList[target] -= handler;

            if (_patrolBroadcastList[target] == null)
            {
                _patrolBroadcastList.Remove(target);
            }
        }
    }


    /// <summary>
    /// 更新所有npc当前巡逻点状态
    /// </summary>
    /// <param name="target"></param>
    /// <param name="point"></param>
    void UpdatePatrolMessage(GameObject target, Transform point)
    {
        // Debug.Log(target.name);
        // Debug.Log(point.name);

        foreach (var VARIABLE in _patrolBroadcastList)
        {
            if (VARIABLE.Key == target)
            {
                VARIABLE.Value(point);
            }
        }
    }

    void UnInit()
    {
        StopAllNpcPatrol();
    }

    private void OnDestroy()
    {
        UnInit();
    }
}
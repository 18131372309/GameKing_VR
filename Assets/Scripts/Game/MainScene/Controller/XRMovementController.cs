using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class XRMovementController : SingleTonMono<XRMovementController>
{
    private InputDevice leftController;
    // private GameObject XROrigin;

    private Vector2 axis;

    private float rate = 1f;
    private float time = 0;

    private Transform XRCameraTrans;
    private Transform XROriginTrans;

    private Vector3 tempXRPosition = Vector3.zero;
    // private float tempZ=0;

    public bool usePhysics = false;

    enum MoveMode
    {
        Translate,
        Character,
        Rigidbody
    }

    enum ViewMode
    {
        Ground,
        Sky
    }

    private ViewMode curViewMode;

    private float groundLimit_Y = 0.5f;
    private float skyLimit_Y = 80;
    private float moveLimt_y = 0;

    private float boundaryLimit_X = 800;
    private float boundaryLimit_Z = 600;

    // Start is called before the first frame update
    void Start()
    {
        InitDevices();
        curViewMode = ViewMode.Ground;
        EventDispatcher.GetInstance().Register("change_view", ChangeViewMode);
        // XRSceneManager.GetInstance.cs += ChangeSceneInit;
        // XROrigin = XROriginInstance.XROriginObj;
        // InvokeRepeating("TestPrintY", 1f, 1);

        if (usePhysics)
        {
            XROriginTrans.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            XROriginTrans.GetComponent<CharacterController>().enabled = false;
        }
    }

    void TestPrintY()
    {
        XRDebug.Log("Y:" + XROriginTrans.position.y);
    }

    void ChangeSceneInit()
    {
        InitDevices();
        //Debug.Log("ChangeSceneInit");
    }

    //TODO:手柄掉綫重連 
    void InitDevices()
    {
       
        leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        XRCameraTrans = XROriginInstance.XROriginObj.GetComponentInChildren<Camera>().transform;
        XROriginTrans = XROriginInstance.XROriginObj.transform;
        
        groundLimit_Y = XROriginTrans.position.y;
        moveLimt_y = groundLimit_Y;
    }

    /// <summary>
    /// 切换视角模式
    /// </summary>
    /// <param name="objs"></param>
    void ChangeViewMode(object[] objs)
    {
        if (curViewMode == ViewMode.Ground)
        {
            tempXRPosition = XROriginTrans.position;
            
            moveLimt_y = skyLimit_Y;
            curViewMode = ViewMode.Sky;
            TeleportXROrigin(new Vector3(0, skyLimit_Y, 0), Quaternion.identity);
        }
        else if (curViewMode == ViewMode.Sky)
        {
            moveLimt_y = groundLimit_Y;
            curViewMode = ViewMode.Ground;
            TeleportXROrigin(tempXRPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TurnCheck();
        TouchMoveCheck();
    }

    /// <summary>
    /// 摇杆移动检测
    /// </summary>
    private void TouchMoveCheck()
    {
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis) && !axis.Equals(Vector2.zero))
        {
            if (axis.y > 0)
            {
                if (usePhysics && curViewMode == ViewMode.Ground)
                {
                    XROriginTrans.GetComponent<CharacterController>()
                        .SimpleMove(XRCameraTrans.forward * 60f * Time.deltaTime);
                }
                else
                {
                    XROriginTrans.Translate(XRCameraTrans.forward * 3.5f * Time.deltaTime);
                }

                // XRDebug.Log(XRCameraTrans.forward.ToString());
                BoundaryCheck();
            }

            //TODO 不要后退
            else
            {
                if (usePhysics && curViewMode == ViewMode.Ground)
                {
                    XROriginTrans.GetComponent<CharacterController>()
                        .SimpleMove(XRCameraTrans.forward * -60f * Time.deltaTime);
                }
                else
                {
                    XROriginTrans.Translate(XRCameraTrans.forward * -3.5f * Time.deltaTime);
                }
                BoundaryCheck();
            }
        }
    }

    /// <summary>
    /// 边界检测
    /// </summary>
    void BoundaryCheck()
    {
        if (!usePhysics && XROriginTrans.position.y != moveLimt_y)
        {
            XROriginTrans.position = new Vector3(XROriginTrans.position.x, moveLimt_y, XROriginTrans.position.z);
        }

        if (XROriginTrans.position.x > boundaryLimit_X / 2)
        {
            XROriginTrans.position =
                new Vector3(boundaryLimit_X / 2, XROriginTrans.position.y, XROriginTrans.position.z);
        }
        else if (XROriginTrans.position.x < boundaryLimit_X / 2 * (-1))
        {
            XROriginTrans.position =
                new Vector3(boundaryLimit_X / 2 * (-1), XROriginTrans.position.y, XROriginTrans.position.z);
        }

        if (XROriginTrans.position.z > boundaryLimit_Z / 2)
        {
            XROriginTrans.position =
                new Vector3(XROriginTrans.position.x, XROriginTrans.position.y, boundaryLimit_Z / 2);
        }
        else if (XROriginTrans.position.z < boundaryLimit_Z / 2 * (-1))
        {
            XROriginTrans.position =
                new Vector3(XROriginTrans.position.x, XROriginTrans.position.y, boundaryLimit_Z / 2 * (-1));
        }
    }


    /// <summary>
    /// 左手遥感旋转检测
    /// </summary>
    /// <returns></returns>
    private void TurnCheck()
    {
        if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis) && !axis.Equals(Vector2.zero))
        {
            // 转头
            if (axis.Equals(Vector2.zero) && (rate - time) > 0.1f)
            {
                time = rate;
            }

            time += Time.deltaTime;
            if (time >= rate)
            {
                if (axis.x < 0)
                {
                    XROriginTrans.Rotate(new Vector3(0, -30, 0));
                    time = 0;
                }
                else
                {
                    XROriginTrans.Rotate(new Vector3(0, 30, 0));
                    time = 0;
                }
            }


            /*和pico不一样，后续再改
           float angle = VectorAngle(new Vector2(1, 0), axis);
           XRDebug.Log(angle.ToString());
           //up
           if (angle>45&&angle<135)
           {
               
           }
           //down
           else if(angle<-45&&angle>-135)
           {
               
           }
           
           //left
           if ((angle < 180 && angle > 135) || (angle < -135 && angle > -180))
           {
               XROrigin.transform.Rotate(new Vector3(0,30,0));
               XRDebug.Log("left");
           }
           //right
           else if ((angle > 0 && angle < 45) || (angle > -45 && angle < 0))
           {
               XROrigin.transform.Rotate(new Vector3(0,-30,0));
               XRDebug.Log("right");
           }
           */
        }
    }

    /// <summary>
    /// 传送XR主体的位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void TeleportXROrigin(Vector3 position, Quaternion rotation)
    {
        //旋转归一吧
        XROriginTrans.SetPositionAndRotation(position, Quaternion.identity);
    }

    float VectorAngle(Vector2 from, Vector2 to)
    {
        float angle;
        Vector3 cross = Vector3.Cross(from, to);
        angle = Vector2.Angle(from, to);
        return cross.z > 0 ? angle : -angle;
    }

    protected override void UnInit()
    {
        base.UnInit();
        EventDispatcher.GetInstance().UnRegister("change_view", ChangeViewMode);
    }
}
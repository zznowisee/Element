using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Connector : Device, IReciever
{
    public ConnectorData data;
    public List<ConnectableDevice> connectingDevices;
    [SerializeField] float radius = 5f;
    [SerializeField] GameObject spriteObj;

    public event Action<Vector3, WarningType> OnWarning;
    public event Action<Connector> OnDestoryByPlayer;

    int recievedMovingCommandNum = 0;

    int totalWaitingNum = 0;
    //recorder:
    Quaternion recorderSpriteObjRotation;

    void Awake()
    {
        transform.Find("sprite").localScale = radius * Vector3.one;
        deviceType = DeviceType.Connector;
        connectingDevices = new List<ConnectableDevice>();
    }

    public override void Setup(HexCell cell_)
    {
        base.Setup(cell_);
        deviceType = DeviceType.Connector;
        data.cellIndex = cell.index;
    }

    void Delay(Action callback, float time)
    {
        StartCoroutine(Sleep(callback, time));
    }

    IEnumerator Sleep(Action callback, float time)
    {
        yield return new WaitForSeconds(time);
        recievedMovingCommandNum = 0;
        callback?.Invoke();
    }

    IEnumerator RotateToTarget(Action callback, RotateDirection rotateDirection, float executeTime)
    {
        yield return null;
        if(recievedMovingCommandNum > 1)
        {
            OnWarning?.Invoke(transform.position, WarningType.ReceiveTwoMoveCommands);
            yield return null;
        }

        float rotateAngle = rotateDirection == RotateDirection.CW ? -60f : 60f;
        float percent = 0f;
        Quaternion start = spriteObj.transform.rotation;
        Quaternion end = Quaternion.Euler(spriteObj.transform.eulerAngles + rotateAngle * Vector3.forward);
        while(percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            spriteObj.transform.rotation = Quaternion.Lerp(start, end, percent);
            yield return null;
        }

        if (connectingDevices.Count == 0)
        {
            recievedMovingCommandNum = 0;
            callback?.Invoke();
        }
    }


    IEnumerator MoveToTarget(Action callback, Direction direction, float executeTime)
    {
        yield return null;
        if(recievedMovingCommandNum > 1)
        {
            OnWarning?.Invoke(transform.position, WarningType.ReceiveTwoMoveCommands);
            yield return null;
        }

        HexCell target = cell.GetNeighbor(direction);
        cell.currentObject = null;

        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            yield return null;
        }

        if (!target.beColoring)
        {
            cell = target;
            cell.currentObject = gameObject;
        }
        else
        {
            OnWarning?.Invoke(transform.position, WarningType.EnteredColoredUnit);
            yield return null;
        }

        if (connectingDevices.Count == 0)
        {
            recievedMovingCommandNum = 0;
            callback?.Invoke();
        }
    }

    public override void Record()
    {
        base.Record();

        recorderSpriteObjRotation = spriteObj.transform.rotation;
    }

    public override void ClearCurrentInfo()
    {
        base.ClearCurrentInfo();
        recievedMovingCommandNum = 0;
        totalWaitingNum = 0;
    }

    public override void ReadPreviousInfo()
    {
        base.ReadPreviousInfo();

        totalWaitingNum = 0;
        connectingDevices.Clear();
        spriteObj.transform.rotation = recorderSpriteObjRotation;
    }

    private void ExecuterFinishedCommand(Action callback)
    {
        totalWaitingNum--;
        if (totalWaitingNum == 0)
        {
            recievedMovingCommandNum = 0;
            callback?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (!ProcessManager.Instance.CanOperate())
            {
                print("Enter");
                OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }

    public void ExecutePutDownUp(Action releaserCallback, float time, BrushState brushState)
    {
        for (int i = 0; i < connectingDevices.Count; i++)
        {
            ConnectableDevice connectableDevice = connectingDevices[i];
            connectableDevice.PutDownUp(releaserCallback, ExecuterFinishedCommand, brushState, time);
        }

        // TODO: Animation
        // without connecting animation
        Delay(releaserCallback, time);
    }

    public void ExecuteConnect(Action releaserCallback, float time)
    {
        List<ConnectableDevice> connectableDevices = cell.GetConnectableDevicesInNeighbor();
        for (int i = 0; i < connectableDevices.Count; i++)
        {
            if (!connectingDevices.Contains(connectableDevices[i]))
            {
                connectableDevices[i].ConnectWithConnector(this);
            }
        }

        // TODO: Animation
        // without connecting animation
        Delay(releaserCallback, time);
    }

    public void ExecuteSplit(Action releaserCallback, float time)
    {
        print(connectingDevices.Count);
        for (int i = 0; i < connectingDevices.Count; i++)
        {
            connectingDevices[i].SplitWithConnector(this);
            i--;
        }

        // TODO: Animation
        // without connecting animation
        Delay(releaserCallback, time);
    }

    public void ExecuteMove(Action releaserCallback, float time, Direction moveDirection)
    {
        StartCoroutine(MoveToTarget(releaserCallback, moveDirection, time));
        totalWaitingNum = connectingDevices.Count;

        for (int i = 0; i < connectingDevices.Count; i++)
        {
            connectingDevices[i].MoveAsChild(releaserCallback, ExecuterFinishedCommand, moveDirection, time);
        }
    }

    public void ExecuteRotate(Action releaserCallback, float time, RotateDirection rotateDirection)
    {
        StartCoroutine(RotateToTarget(releaserCallback, rotateDirection, time));
        totalWaitingNum = connectingDevices.Count;

        for (int i = 0; i < connectingDevices.Count; i++)
        {
            HexCell target = rotateDirection == RotateDirection.CCW ? cell.PreviousCell(connectingDevices[i].cell) : cell.NextCell(connectingDevices[i].cell);
            connectingDevices[i].MoveAsChild(releaserCallback, ExecuterFinishedCommand, target, time);
        }
    }

    public override void DestoryDevice()
    {
        OnDestoryByPlayer?.Invoke(this);
        base.DestoryDevice();
    }
}

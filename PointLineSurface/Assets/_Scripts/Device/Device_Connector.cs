using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device_Connector : Device, IReciever
{
    Device_WaitingChecker waitingChecker;
    public Data_Connector data;
    public List<ConnectableDevice> connectingDevices;
    [SerializeField] float radius = 5f;
    [SerializeField] GameObject spriteObj;

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

    public void Rebuild(Data_Connector data_)
    {
        data = data_;
        Setup(MapGenerator.Instance.GetHexCell(data.index));
    }

    public override void Setup(HexCell cell_)
    {
        base.Setup(cell_);
        data.index = cell.hexCoordinates.GetValue();
    }

    void Delay(Action callback, float time)
    {
        StartCoroutine(Sleep(callback, time));

        IEnumerator Sleep(Action callback, float time)
        {
            yield return new WaitForSeconds(time);
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
            waitingChecker.CheckFinish();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (!ProcessManager.Instance.CanOperate())
            {
                UI_WarningManager.ShowWarning(transform.position, WarningType.Collision);
            }
        }
    }

    public void ExecutePutdown(Action releaserCallback, float time)
    {
        for (int i = 0; i < connectingDevices.Count; i++)
        {
            connectingDevices[i].Putdown(releaserCallback, ExecuterFinishedCommand, time);
        }

        Delay(releaserCallback, time);
    }

    public void ExecutePutup(Action releaserCallback, float time)
    {
        for (int i = 0; i < connectingDevices.Count; i++)
        {
            ConnectableDevice connectableDevice = connectingDevices[i];
            connectableDevice.Putup(releaserCallback, ExecuterFinishedCommand, time);
        }

        // TODO: Animation
        // without connecting animation
        Delay(releaserCallback, time);
    }

    public void ExecuteConnect(Action releaserCallback, float time)
    {
        List<ConnectableDevice> connectableDevices = cell.GetConnectableDeviceListInNeighbor();
        for (int i = 0; i < connectableDevices.Count; i++)
        {
            if (!connectingDevices.Contains(connectableDevices[i]))
            {
                if (connectableDevices[i].connectingConnector != null && connectableDevices[i].beConnectedThisFrame)
                {
                    UI_WarningManager.ShowWarning(transform.position, WarningType.ConnectedByTwo);
                    // warning.
                }
                else if(connectableDevices[i].connectingConnector == null)
                {
                    connectableDevices[i].ConnectWithConnector(this);
                }
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

    public void ExecuteMove(Action releaserCallback, float time, Direction dir)
    {
        StartCoroutine(MoveToTarget(releaserCallback, dir, time));
        totalWaitingNum = connectingDevices.Count;

        for (int i = 0; i < connectingDevices.Count; i++)
        {
            connectingDevices[i].MoveAsChild(releaserCallback, ExecuterFinishedCommand, dir, time);
        }

        IEnumerator MoveToTarget(Action callback, Direction direction, float executeTime)
        {
            yield return null;
            if (recievedMovingCommandNum > 1)
            {
                UI_WarningManager.ShowWarning(transform.position, WarningType.ReceiveTwoMoveCommands);
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

            cell = target;
            cell.currentObject = gameObject;
            target.EraseColor();

            if (connectingDevices.Count == 0)
            {
                recievedMovingCommandNum = 0;
                callback?.Invoke();
            }
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

        IEnumerator RotateToTarget(Action callback, RotateDirection rotateDirection, float executeTime)
        {
            yield return null;
            if (recievedMovingCommandNum > 1)
            {
                UI_WarningManager.ShowWarning(transform.position, WarningType.ReceiveTwoMoveCommands);
                yield return null;
            }

            float rotateAngle = rotateDirection == RotateDirection.CW ? -60f : 60f;
            float percent = 0f;
            Quaternion start = spriteObj.transform.rotation;
            Quaternion end = Quaternion.Euler(spriteObj.transform.eulerAngles + rotateAngle * Vector3.forward);
            while (percent < 1f)
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
    }
}

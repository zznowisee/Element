using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Connector : Device, IMouseAction, IReciever
{
    public ConnectorData connectorData;

    public List<ConnectableDevice> connectingDevices;
    [SerializeField] float radius = 5f;
    [SerializeField] GameObject spriteObj;

    public event Action<Vector3, WarningType> OnWarning;

    int recievedMovingCommandNum = 0;

    int totalWaitingNum = 0;
    //recorder:
    HexCell recorderCell;
    Quaternion recorderSpriteObjRotation;

    void Awake()
    {
        transform.Find("sprite").localScale = radius * Vector3.one;

        connectingDevices = new List<ConnectableDevice>();
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;
        connectorData.cellIndex = cell.index;
    }

    public void OnSwitchToMainScene()
    {
        cell.currentObject = null;
        Destroy(gameObject);
    }

    public void MouseAction_Drag()
    {
        cell.currentObject = null;
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingConnector(this);
    }

    IEnumerator Sleep(Action callback)
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.defaultExecuteTime);
        recievedMovingCommandNum = 0;
        callback?.Invoke();
    }

    IEnumerator RotateToTarget(Action callback, RotateDirection rotateDirection)
    {
        yield return null;

        if(recievedMovingCommandNum > 1)
        {
            StopAllCoroutines();
            totalWaitingNum = 0;
            OnWarning?.Invoke(transform.position, WarningType.ReceiveTwoMoveCommands);
            yield return null;
        }

        float rotateAngle = rotateDirection == RotateDirection.CW ? -60f : 60f;
        float percent = 0f;
        Quaternion start = spriteObj.transform.rotation;
        Quaternion end = Quaternion.Euler(spriteObj.transform.eulerAngles + rotateAngle * Vector3.forward);
        while(percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.defaultExecuteTime;
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

    IEnumerator MoveToTarget(Action callback, Direction direction)
    {
        yield return null;
        if(recievedMovingCommandNum > 1)
        {
            StopAllCoroutines();
            totalWaitingNum = 0;
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
            percent += Time.deltaTime / ProcessSystem.Instance.defaultExecuteTime;
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
        recorderCell = cell;
        recorderSpriteObjRotation = spriteObj.transform.rotation;
    }

    public override void ClearCurrentInfo()
    {
        recievedMovingCommandNum = 0;
        totalWaitingNum = 0;
        cell.currentObject = null;
    }

    public override void ReadPreviousInfo()
    {
        cell = recorderCell;
        cell.currentObject = gameObject;
        recorderCell = null;
        totalWaitingNum = 0;
        connectingDevices.Clear();
        transform.position = cell.transform.position;
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
            if (!ProcessSystem.Instance.CanOperate())
            {
                print("Enter");
                OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }

    public void ExecutePutDownUp(Action releaserCallback, float time, BrushState brushState)
    {
        throw new NotImplementedException();
    }

    public void ExecuteConnect(Action releaserCallback, float time)
    {
        throw new NotImplementedException();
    }

    public void ExecuteSplit(Action releaserCallback, float time)
    {
        throw new NotImplementedException();
    }

    public void ExecuteMove(Action releaserCallback, float time, Direction moveDirection)
    {
        throw new NotImplementedException();
    }

    public void ExecuteRotate(Action releaserCallback, float time, RotateDirection rotateDirection)
    {
        throw new NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public enum RotateDirection
{
    CW,
    CCW
}

public class Controller : Device, IMouseAction, ICommandReleaser, IReciever
{
    public Direction direction;
    public ControllerData controllerData;
    public ControllerBtn controllerBtn;
    [SerializeField] float predictionLineLength = 200f;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] LineRenderer predictionLine;
    [SerializeField] GameObject sprite;

    public event Action OnMouseDragBegin;
    public event Action OnMouseDragEnd;

    public event Action OnFinishedOneLineCommand;

    public event Action<Vector3, WarningType> OnWarning;

    //recorder:
    Direction recorderDirection;
    Quaternion recorderSpriteRotation;

    int recievedMovingCommandNum = 0;
    int totalWaitingNum = 0;

    void Awake()
    {
        predictionLine.gameObject.SetActive(false);
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);
    }

    public void OnSwitchToMainScene()
    {
        cell.currentObject = null;
        Destroy(gameObject);
    }

    void UpdatePredictionLine()
    {
        predictionLine.SetPositions(new Vector3[]
        {
            Vector3.zero,
            Vector3.up * predictionLineLength
        });
    }

    public void Setup(HexCell cell, ControllerData controllerData_)
    {
        controllerData = controllerData_;
        direction = controllerData.direction;
        SetIndex(controllerData.consoleIndex);
        Setup(cell);
    }
    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;

        transform.position = cell.transform.position;

        predictionLine.gameObject.SetActive(true);
        UpdatePredictionLine();
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -(int)direction * 60f - Vector3.forward * 30f);
        controllerData.cellIndex = cell.index;
    }

    public void SetIndex(int index_)
    {
        index = index_;
        indexText.text = index.ToString();
        indexText.gameObject.SetActive(true);
        controllerData.consoleIndex = index;
    }

    public void MouseAction_Drag()
    {
        predictionLine.gameObject.SetActive(false);
        cell.currentObject = null;

        BuildSystem.Instance.SetCurrentTrackingController(this);
        OnMouseDragBegin?.Invoke();
    }

    public void Rotate(RotateDirection rotateDirection)
    {
        switch (rotateDirection)
        {
            //ccw
            case RotateDirection.CCW:
                direction = direction.Previous();
                sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                break;
            //cw
            case RotateDirection.CW:
                direction = direction.Next();
                sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                break;
        }
        controllerData.direction = direction;
    }

    private void OnRecieverFinishedCommand()
    {
        totalWaitingNum--;
        if (totalWaitingNum == 0)
        {
            OnFinishedOneLineCommand?.Invoke();
        }
    }

    public void ReleaseCommand(CommandType commandType, float executeTime)
    {
        switch (commandType)
        {
            case CommandType.ControllerCCR:
                ControllerCCWRotate();
                return;
            case CommandType.ControllerCR:
                ControllerCWRotate();
                return;
            case CommandType.Delay:
                Delay(executeTime);
                return;
        }

        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                IReciever reciever = next.GetICommandReciever();
                if (reciever != null)
                {
                    totalWaitingNum++;

                    switch (commandType)
                    {
                        case CommandType.Connect:
                            reciever.ExecuteConnect(RecieverFinishedCommand, executeTime);
                            break;
                        case CommandType.ConnectorCCW:
                            reciever.ExecuteRotate(RecieverFinishedCommand, executeTime, RotateDirection.CCW);
                            break;
                        case CommandType.ConnectorCW:
                            reciever.ExecuteRotate(RecieverFinishedCommand, executeTime, RotateDirection.CW);
                            break;
                        case CommandType.Pull:
                            reciever.ExecuteMove(RecieverFinishedCommand, executeTime, direction.Opposite());
                            break;
                        case CommandType.Push:
                            reciever.ExecuteMove(RecieverFinishedCommand, executeTime, direction);
                            break;
                        case CommandType.PutDown:
                            reciever.ExecutePutDownUp(RecieverFinishedCommand, executeTime, BrushState.PUTDOWN);
                            break;
                        case CommandType.PutUp:
                            reciever.ExecutePutDownUp(RecieverFinishedCommand, executeTime, BrushState.PUTUP);
                            break;
                        case CommandType.Split:
                            reciever.ExecuteSplit(RecieverFinishedCommand, executeTime);
                            break;
                    }
                }
                next = next.GetNeighbor(direction);
            }
            else
            {
                break;
            }
        }

        if (totalWaitingNum == 0)
        {
            Delay(executeTime);
        }
    }

    void Delay(float time)
    {
        StartCoroutine(Sleep(time));
    }

    public override void ClearCurrentInfo()
    {
        cell.currentObject = null;
        totalWaitingNum = 0;
        recievedMovingCommandNum = 0;
    }

    public override void ReadPreviousInfo()
    {
        cell = recordCell;

        cell.currentObject = gameObject;
        direction = recorderDirection;
        UpdatePredictionLine();
        recordCell = null;
        sprite.transform.rotation = recorderSpriteRotation;
        recorderSpriteRotation = Quaternion.identity;
        transform.position = cell.transform.position;
    }

    public override void Record()
    {
        recordCell = cell;
        recorderDirection = direction;
        recorderSpriteRotation = sprite.transform.rotation;
    }

    IEnumerator Sleep(float executeTime)
    {
        yield return new WaitForSeconds(executeTime);
        recievedMovingCommandNum = 0;
        OnFinishedOneLineCommand?.Invoke();
    }

    IEnumerator MoveToTarget(Action callback, Direction direction)
    {
        yield return null;
        if (recievedMovingCommandNum > 1)
        {
            StopAllCoroutines();
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
        }

        callback?.Invoke();
        recievedMovingCommandNum = 0;
    }

    public void ControllerCCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.CCW));
    public void ControllerCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.CW));
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

    IEnumerator RotateToTarget(RotateDirection rotateDirection)
    {
        Quaternion start = sprite.transform.rotation;
        Quaternion end = start;
        float percent = 0f;
        switch (rotateDirection)
        {
            case RotateDirection.CW:
                end = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                direction = direction.Next();
                break;
            case RotateDirection.CCW:
                end = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                direction = direction.Previous();
                break;
        }

        while(percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.defaultExecuteTime;
            percent = Mathf.Clamp01(percent);
            sprite.transform.rotation = Quaternion.Lerp(start, end, percent);
            yield return null;
        }

        recievedMovingCommandNum = 0;
        OnFinishedOneLineCommand?.Invoke();
    }

    public void RecieverFinishedCommand()
    {
        
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

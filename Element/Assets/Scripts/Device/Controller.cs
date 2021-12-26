using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public enum RotateDirection
{
    Clockwise,
    CounterClockwise
}

public class Controller : CommandRunner, IMouseAction, ICommandReader, ICommandReciever
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

    public event Action OnFinishCommand;

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
            case RotateDirection.CounterClockwise:
                direction = direction.Previous();
                sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                break;
            //cw
            case RotateDirection.Clockwise:
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
            OnFinishCommand?.Invoke();
        }
    }
    public void RunCommand(CommandType commandType)
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
                StartCoroutine(Sleep());
                return;
        }

        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                ICommandReciever reciever = next.GetICommandReciever();
                if (reciever != null)
                {
                    totalWaitingNum++;

                    switch (commandType)
                    {
                        case CommandType.Connect:
                            reciever.RunConnect(OnRecieverFinishedCommand);
                            break;
                        case CommandType.ConnectorCCR:
                            reciever.RunRotate(OnRecieverFinishedCommand, RotateDirection.CounterClockwise);
                            break;
                        case CommandType.ConnectorCR:
                            reciever.RunRotate(OnRecieverFinishedCommand, RotateDirection.Clockwise);
                            break;
                        case CommandType.Pull:
                            reciever.RunMove(OnRecieverFinishedCommand, direction.Opposite());
                            break;
                        case CommandType.Push:
                            reciever.RunMove(OnRecieverFinishedCommand, direction);
                            break;
                        case CommandType.PutDown:
                            reciever.RunPutDownUp(OnRecieverFinishedCommand, true);
                            break;
                        case CommandType.PutUp:
                            reciever.RunPutDownUp(OnRecieverFinishedCommand, false);
                            break;
                        case CommandType.Split:
                            reciever.RunSplit(OnRecieverFinishedCommand);
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
            StartCoroutine(Sleep());
        }
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

    IEnumerator Sleep()
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        recievedMovingCommandNum = 0;
        OnFinishCommand?.Invoke();
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
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
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

    public void ControllerCCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.CounterClockwise));
    public void ControllerCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.Clockwise));
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
            case RotateDirection.Clockwise:
                end = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                direction = direction.Next();
                break;
            case RotateDirection.CounterClockwise:
                end = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                direction = direction.Previous();
                break;
        }

        while(percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
            percent = Mathf.Clamp01(percent);
            sprite.transform.rotation = Quaternion.Lerp(start, end, percent);
            yield return null;
        }

        recievedMovingCommandNum = 0;
        OnFinishCommand?.Invoke();
    }

    public void RunMove(Action callback, Direction moveDirection)
    {
        recievedMovingCommandNum++;
        StartCoroutine(MoveToTarget(callback, moveDirection));
    }
    public void RunPutDownUp(Action callback, bool coloring) => callback?.Invoke();
    public void RunRotate(Action callback, RotateDirection rotateDirection) => callback?.Invoke();
    public void RunConnect(Action callback) => callback?.Invoke();
    public void RunSplit(Action callback) => callback?.Invoke();
    public void RunDelay(Action callback) => callback?.Invoke();
}

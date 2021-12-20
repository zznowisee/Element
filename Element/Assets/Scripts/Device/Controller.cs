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

public class Controller : CommandRunner, IMouseDrag, ICommandReader, ICommandReciever
{
    public Direction direction;
    public ControllerData controllerData;

    [SerializeField] float predictionLineLength = 200f;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] LineRenderer predictionLine;
    [SerializeField] GameObject sprite;

    public event Action OnMouseDragBegin;
    public event Action OnMouseDragEnd;
    public event Action OnFinishCommand;
    public event Action OnFinishSecondLevelCommand;
    public event Action<Vector3, WarningType> OnWarning;

    //recorder:
    Direction recorderDirection;
    Quaternion recorderSpriteRotation;

    List<ICommandReciever> recievers;
    [HideInInspector] public int connectorCommandCounter;
    [HideInInspector] public int recievedMovingCommandNum = 0;

    void Awake()
    {
        predictionLine.gameObject.SetActive(false);
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);
        recievers = new List<ICommandReciever>();
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

    public void StartDragging()
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

    private void OnConnectorFinishCommand()
    {
        print("OnConnectorFinishCommand");
        connectorCommandCounter++;
        if (connectorCommandCounter == recievers.Count)
        {
            for (int i = 0; i < recievers.Count; i++)
            {
                recievers[i].OnFinishSecondLevelCommand -= OnConnectorFinishCommand;
            }
            connectorCommandCounter = 0;
            recievers.Clear();
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
        }

        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                ICommandReciever reciever = next.GetICommandReciever();
                if (reciever != null)
                {
                    switch (commandType)
                    {
                        case CommandType.Connect:
                            reciever.RunConnect();
                            break;
                        case CommandType.ConnectorCCR:
                            reciever.RunRotate(RotateDirection.CounterClockwise);
                            break;
                        case CommandType.ConnectorCR:
                            reciever.RunRotate(RotateDirection.Clockwise);
                            break;
                        case CommandType.Delay:
                            reciever.RunDelay();
                            break;
                        case CommandType.Pull:
                            reciever.RunMove(direction.Opposite());
                            break;
                        case CommandType.Push:
                            reciever.RunMove(direction);
                            break;
                        case CommandType.PutDown:
                            reciever.RunPutDownUp(true);
                            break;
                        case CommandType.PutUp:
                            reciever.RunPutDownUp(false);
                            break;
                        case CommandType.Split:
                            reciever.RunSplit();
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

        if (recievers.Count == 0)
        {
            StartCoroutine(Sleep());
        }
    }


    public override void ClearCurrentInfo()
    {
        cell.currentObject = null;
        for (int i = 0; i < recievers.Count; i++)
        {
            recievers[i].OnFinishSecondLevelCommand -= OnConnectorFinishCommand;
        }
        recievers.Clear();
        connectorCommandCounter = 0;
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
        connectorCommandCounter = 0;
        recievedMovingCommandNum = 0;
        OnFinishCommand?.Invoke();
    }

    IEnumerator MoveToTarget(Direction direction)
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
            recievedMovingCommandNum = 0;
        }
        else
        {
            OnWarning?.Invoke(transform.position, WarningType.EnteredColoredUnit);
        }

        OnFinishSecondLevelCommand?.Invoke();
        if(recievers.Count == 0)
        {
            recievedMovingCommandNum = 0;
            connectorCommandCounter = 0;
            OnFinishCommand?.Invoke();
        }
    }

    public void RunPutDownUp(bool coloring)
    {
        OnFinishCommand?.Invoke();
    }

    public void RunRotate(RotateDirection rotateDirection)
    {
        recievedMovingCommandNum++;
        OnFinishCommand?.Invoke();
    }

    public void RunMove(Direction moveDirection)
    {
        recievedMovingCommandNum++;
        StartCoroutine(MoveToTarget(moveDirection));
    }
    public void ControllerCCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.CounterClockwise));
    public void ControllerCWRotate() => StartCoroutine(RotateToTarget(RotateDirection.Clockwise));
    public void RunConnect() => OnFinishCommand?.Invoke();
    public void RunSplit() => OnFinishCommand?.Invoke();
    public void RunDelay() => OnFinishCommand?.Invoke();

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
        connectorCommandCounter = 0;
        recievedMovingCommandNum = 0;
        OnFinishCommand?.Invoke();
    }
}

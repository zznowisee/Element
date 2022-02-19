using System.Collections;
using UnityEngine;
using System;
using TMPro;

public enum RotateDirection
{
    CW,
    CCW
}

public class Controller : Device, ICommandReleaser, IReciever
{
    public Direction direction;
    public ControllerData data;
    [SerializeField] float predictionLineLength = 200f;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] LineRenderer predictionLine;
    [SerializeField] GameObject sprite;

    public event Action<Controller> OnDestoryByPlayer;
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
        deviceType = DeviceType.Controller;
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);
    }

    void UpdatePredictionLine()
    {
        predictionLine.SetPositions(new Vector3[]
        {
            Vector3.zero,
            Vector3.up * predictionLineLength
        });
    }

    public void Rebuild(HexCell cell_, ControllerData data_)
    {
        data = data_;
        direction = data.direction;
        UpdatePredictionLine();
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -(int)direction * 60f - Vector3.forward * 30f);
        SetIndex(data.index);
        Setup(cell_);
    }
    public override void Setup(HexCell cell_)
    {
        base.Setup(cell_);
        deviceType = DeviceType.Controller;
        predictionLine.gameObject.SetActive(true);
        UpdatePredictionLine();
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -(int)direction * 60f - Vector3.forward * 30f);
        data.cellIndex = cell.index;
    }

    public void SetIndex(int index_)
    {
        index = index_;
        indexText.text = index.ToString();
        indexText.gameObject.SetActive(true);
        data.index = index;
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
        data.direction = direction;
    }

    public void ReleaseCommand(CommandType commandType, float executeTime)
    {
        switch (commandType)
        {
            case CommandType.ControllerCCW:
                ControllerCCWRotate(executeTime);
                return;
            case CommandType.ControllerCW:
                ControllerCWRotate(executeTime);
                return;
            case CommandType.Delay:
                Delay(OnFinishCommand, executeTime);
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
            Delay(OnFinishCommand, executeTime);
        }
    }

    void Delay(Action callback, float time)
    {
        StartCoroutine(Sleep(callback, time));
    }

    public override void ClearCurrentInfo()
    {
        base.ClearCurrentInfo();

        totalWaitingNum = 0;
        recievedMovingCommandNum = 0;
    }

    public override void ReadPreviousInfo()
    {
        base.ReadPreviousInfo();

        direction = recorderDirection;
        UpdatePredictionLine();
        sprite.transform.rotation = recorderSpriteRotation;
        recorderSpriteRotation = Quaternion.identity;
    }

    public override void Record()
    {
        base.Record();
        recorderDirection = direction;
        recorderSpriteRotation = sprite.transform.rotation;
    }

    IEnumerator Sleep(Action callback, float executeTime)
    {
        yield return new WaitForSeconds(executeTime);
        recievedMovingCommandNum = 0;
        callback?.Invoke();
    }

    IEnumerator MoveToTarget(Action callback, Direction direction, float executeTime)
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
        }

        callback?.Invoke();
        recievedMovingCommandNum = 0;
    }

    public void ControllerCCWRotate(float executeTime) => StartCoroutine(RotateToTarget(executeTime, RotateDirection.CCW));
    public void ControllerCWRotate(float executeTime) => StartCoroutine(RotateToTarget(executeTime, RotateDirection.CW));
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

    IEnumerator RotateToTarget(float time, RotateDirection rotateDirection)
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
            percent += Time.deltaTime / time;
            percent = Mathf.Clamp01(percent);
            sprite.transform.rotation = Quaternion.Lerp(start, end, percent);
            yield return null;
        }

        recievedMovingCommandNum = 0;
        OnFinishCommand?.Invoke();
    }

    public void RecieverFinishedCommand()
    {
        totalWaitingNum--;
        if (totalWaitingNum == 0)
        {
            print("Controller Finish");
            OnFinishCommand?.Invoke();
        }
    }

    public void ExecutePutDownUp(Action releaserCallback, float time, BrushState brushState)
    {
        releaserCallback?.Invoke();
    }

    public void ExecuteConnect(Action releaserCallback, float time)
    {
        releaserCallback?.Invoke();
    }

    public void ExecuteSplit(Action releaserCallback, float time)
    {
        releaserCallback?.Invoke();
    }

    public void ExecuteMove(Action releaserCallback, float time, Direction moveDirection)
    {
        StartCoroutine(MoveToTarget(releaserCallback, moveDirection, time));
    }

    public void ExecuteRotate(Action releaserCallback, float time, RotateDirection rotateDirection)
    {
        releaserCallback?.Invoke();
    }

    public override void LeftClick()
    {
        base.LeftClick();
        predictionLine.gameObject.SetActive(false);
    }

    public override void Dragging()
    {
        base.Dragging();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(RotateDirection.CCW);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(RotateDirection.CW);
        }
    }

    public override void DestoryDevice()
    {
        print("Destory Device");
        OnDestoryByPlayer?.Invoke(this);
        base.DestoryDevice();
    }
}

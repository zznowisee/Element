using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class Device_Controller : Device, IReleaser, IReciever
{
    Device_WaitingChecker callbacker;
    public int Index
    {
        get { return data.orderIndex; }
    }

    public Data_Controller data;
    const int maxCoverNumber = 7;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] LineRenderer predictionLine;
    [SerializeField] GameObject sprite;
    public event Action OnFinishCommand;

    //recorder:
    Direction recorderDirection;
    Quaternion recorderSpriteRotation;

    int recievedMovingCommandNum = 0;
    int totalWaitingNum = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        OnDestoryByPlayer += DataManager.Instance.RemoveDeviceData;
        OnDestory += DeviceManager.Instance.ResetControllerIndices;
        OnDestoryByPlayer += UI_OperateScene.Instance.UpdateControllerBtnNumber;

        predictionLine.gameObject.SetActive(false);
        deviceType = DeviceType.Controller;
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);

        callbacker = new Device_WaitingChecker(OnFinishCommand);
    }

    void UpdatePredictionLine()
    {
        predictionLine.SetPositions(new Vector3[]
        {
            Vector3.zero,
            Vector3.up * HexMatrix.innerRadius * maxCoverNumber * 2
        });
    }

    public void Rebuild(Data_Controller data_)
    {
        data = data_;
        UpdatePredictionLine();
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -(int)data.direction * 60f - Vector3.forward * 30f);
        Init(data.orderIndex);
        Setup(MapGenerator.Instance.GetHexCell(data.index.x, data.index.y));
    }
    public override void Setup(HexCell cell_)
    {
        //data.deviceType = DeviceType.Controller;
        base.Setup(cell_);
        predictionLine.gameObject.SetActive(true);
        UpdatePredictionLine();
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -(int)data.direction * 60f - Vector3.forward * 30f);
        data.index = cell.hexCoordinates.GetValue();
    }

    public void Init(int index_)
    {
        data.orderIndex = index_;
        indexText.text = data.orderIndex.ToString();
        indexText.gameObject.SetActive(true);
    }

    public void ReleaseCommand(CommandType commandType, float executeTime)
    {
        print("ReleaseCommand");
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
            case CommandType.Lock:
                print("Lock.");
                return;
            case CommandType.Unlock:
                print("Unlock.");
                return;
        }

        int leftCheckNumber = maxCoverNumber;
        Direction direction = data.direction;
        HexCell next = cell.GetNeighbor(direction);
        print("Next is null.");
        do
        {
            if (next)
            {
                IReciever reciever = next.GetICommandReciever();
                Device_Extender extender = next.GetExtender();
                if (reciever != null)
                {
                    totalWaitingNum++;

                    switch (commandType)
                    {
                        case CommandType.Connect: reciever.ExecuteConnect(RecieverFinishedCommand, executeTime); break;
                        case CommandType.Split: reciever.ExecuteSplit(RecieverFinishedCommand, executeTime); break;
                        case CommandType.ConnectorCCW: reciever.ExecuteRotate(RecieverFinishedCommand, executeTime, RotateDirection.CCW); break;
                        case CommandType.ConnectorCW: reciever.ExecuteRotate(RecieverFinishedCommand, executeTime, RotateDirection.CW); break;
                        case CommandType.Pull: reciever.ExecuteMove(RecieverFinishedCommand, executeTime, direction.Opposite()); break;
                        case CommandType.Push: reciever.ExecuteMove(RecieverFinishedCommand, executeTime, direction); break;
                        case CommandType.PutDown: reciever.ExecutePutdown(RecieverFinishedCommand, executeTime); break;
                        case CommandType.PutUp: reciever.ExecutePutup(RecieverFinishedCommand, executeTime); break;
                    }
                }
                else if (extender != null)
                {
                    leftCheckNumber = maxCoverNumber + 1;
                    direction = extender.data.direction;
                }
                next = next.GetNeighbor(direction);
            }
            leftCheckNumber--;
        } while (leftCheckNumber != 0);

        if (totalWaitingNum == 0)
        {
            Delay(OnFinishCommand, executeTime);
        }
    }

    void Delay(Action callback, float time)
    {
        StartCoroutine(Sleep());

        IEnumerator Sleep()
        {
            yield return new WaitForSeconds(time);
            recievedMovingCommandNum = 0;
            callback?.Invoke();
        }
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

        data.direction = recorderDirection;
        UpdatePredictionLine();
        sprite.transform.rotation = recorderSpriteRotation;
        recorderSpriteRotation = Quaternion.identity;
    }

    public override void Record()
    {
        base.Record();
        recorderDirection = data.direction;
        recorderSpriteRotation = sprite.transform.rotation;
    }

    public void ControllerCCWRotate(float executeTime) => StartCoroutine(RotateToTarget(executeTime, RotateDirection.CCW));
    public void ControllerCWRotate(float executeTime) => StartCoroutine(RotateToTarget(executeTime, RotateDirection.CW));
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

    IEnumerator RotateToTarget(float time, RotateDirection rotateDirection)
    {
        Quaternion start = sprite.transform.rotation;
        Quaternion end = start;
        float percent = 0f;
        switch (rotateDirection)
        {
            case RotateDirection.CW:
                end = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                data.direction = data.direction.Next();
                break;
            case RotateDirection.CCW:
                end = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                data.direction = data.direction.Previous();
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

    public void ExecutePutup(Action releaserCallback, float time)
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

        IEnumerator MoveToTarget(Action callback, Direction direction, float executeTime)
        {
            yield return null;
            if (recievedMovingCommandNum > 1)
            {
                StopAllCoroutines();
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
            cell.EraseColor();

            callback?.Invoke();
            recievedMovingCommandNum = 0;
        }
    }

    public void ExecutePutdown(Action releaserCallback, float time)
    {
        releaserCallback?.Invoke();
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

        void Rotate(RotateDirection rotateDirection)
        {
            switch (rotateDirection)
            {
                //ccw
                case RotateDirection.CCW:
                    data.direction = data.direction.Previous();
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                    break;
                //cw
                case RotateDirection.CW:
                    data.direction = data.direction.Next();
                    sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                    break;
            }
        }
    }
}

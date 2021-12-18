using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Connector : MonoBehaviour, IMouseDrag, ICommandReciever
{
    public ConnectorData connectorData;

    public List<Brush> brushes;
    public HexCell cell;
    [SerializeField] float radius = 5f;
    [SerializeField] GameObject spriteObj;

    public event Action<Connector, Direction> OnMoveActionStart;
    public event Action<Connector, int> OnRotateActionStart;
    public event Action OnSleepActionStart;
    public event Action OnFinishSecondLevelCommand;
    public event Action<Vector3, WarningType> OnWarning;

    public int brushFinishCounter = 0;
    public int recievedMovingCommandNum = 0;

    //recorder:
    HexCell recorderCell;
    Quaternion recorderSpriteObjRotation;

    void Awake()
    {
        transform.Find("sprite").localScale = radius * Vector3.one;
        brushes = new List<Brush>();
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.connector = this;
        cell.reciever = this;
        transform.position = cell.transform.position;
        connectorData.cellIndex = cell.index;
    }

    public void OnSwitchToMainScene()
    {
        cell.connector = null;
        cell.reciever = null;
        Destroy(gameObject);
    }

    public void StartDragging()
    {
        cell.connector = null;
        cell.reciever = null;
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingConnector(this);
    }

    IEnumerator Sleep()
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        brushFinishCounter = 0;
        recievedMovingCommandNum = 0;
        OnFinishSecondLevelCommand?.Invoke();
    }

    IEnumerator RotateToTarget(int rotateIndex)
    {
        yield return null;
        if(recievedMovingCommandNum > 1)
        {
            OnWarning?.Invoke(transform.position, WarningType.ReceiveTwoMoveCommands);
            yield return null;
        }
        else
        {
            OnRotateActionStart?.Invoke(this, rotateIndex);
        }

        float rotateAngle = rotateIndex == 1 ? -60f : 60f;
        float percent = 0f;
        Quaternion start = spriteObj.transform.rotation;
        Quaternion end = Quaternion.Euler(spriteObj.transform.eulerAngles + rotateAngle * Vector3.forward);
        while(percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
            percent = Mathf.Clamp01(percent);
            spriteObj.transform.rotation = Quaternion.Lerp(start, end, percent);
            yield return null;
        }

        if (brushes.Count == 0)
        {
            recievedMovingCommandNum = 0;
            brushFinishCounter = 0;
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    IEnumerator MoveToTarget(Direction direction)
    {
        yield return null;
        if(recievedMovingCommandNum > 1)
        {
            StopAllCoroutines();
            OnWarning?.Invoke(transform.position, WarningType.ReceiveTwoMoveCommands);
            yield return null;
        }
        else
        {
            OnMoveActionStart?.Invoke(this, direction);
        }

        HexCell target = cell.GetNeighbor(direction);
        cell.connector = null;
        cell.reciever = null;

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
            cell.connector = this;
            cell.reciever = this;
        }
        else
        {
            OnWarning?.Invoke(transform.position, WarningType.EnteredColoredUnit);
        }

        if (brushes.Count == 0)
        {
            recievedMovingCommandNum = 0;
            brushFinishCounter = 0;
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    public void Record()
    {
        recorderCell = cell;
        recorderSpriteObjRotation = spriteObj.transform.rotation;
    }

    public void ClearCurrentInfo()
    {
        recievedMovingCommandNum = 0;
        brushFinishCounter = 0;
        cell.connector = null;
        cell.reciever = null;
    }

    public void ReadPreviousInfo()
    {
        cell = recorderCell;
        cell.connector = this;
        cell.reciever = this;
        recorderCell = null;

        for (int i = 0; i < brushes.Count; i++)
        {
            if (brushes[i] != null)
            {
                brushes[i].OnFinishThirdLevelCommand -= OnBrushFinishCommand;
                brushes[i] = null;
                brushes.RemoveAt(i);
                i--;
            }
        }
        transform.position = cell.transform.position;
        spriteObj.transform.rotation = recorderSpriteObjRotation;
    }

    public void RunPutDownUp(bool coloring)
    {
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].PutDownUp(coloring);
        }

        if (brushes.Count == 0)
        {
            StartCoroutine(Sleep());
        }
    }

    public void RunConnect()
    {
        OnSleepActionStart?.Invoke();

        for (int i = 0; i < cell.neighbors.Length; i++)
        {
            if(cell.neighbors[i] != null)
            {
                if(cell.neighbors[i].brush != null)
                {
                    Brush newBrush = cell.neighbors[i].brush;
                    if (!brushes.Contains(newBrush))
                    {
                        brushes.Add(newBrush);
                        newBrush.ConnectWithConnector(this);
                        newBrush.OnFinishThirdLevelCommand += OnBrushFinishCommand;
                    }
                }
            }
        }

        if(brushes.Count == 0)
        {
            StartCoroutine(Sleep());
        }
    }

    public void RunSplit()
    {
        for (int i = 0; i < brushes.Count; i++)
        {
            if (brushes[i] != null)
            {
                brushes[i].OnFinishThirdLevelCommand -= OnBrushFinishCommand;
                brushes[i].SplitWithConnector(this);
                brushes[i] = null;
                brushes.RemoveAt(i);
                i--;
            }
        }

        StartCoroutine(Sleep());
    }

    private void OnBrushFinishCommand()
    {
        brushFinishCounter++;
        if(brushFinishCounter == brushes.Count)
        {
            brushFinishCounter = 0;
            recievedMovingCommandNum = 0;
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    public void RunRotate(int rotateDirection)
    {
        recievedMovingCommandNum++;
        StartCoroutine(RotateToTarget(rotateDirection));
    }

    public void RunMove(Direction moveDirection)
    {
        recievedMovingCommandNum++;
        StartCoroutine(MoveToTarget(moveDirection));
    }

    public void RunDelay()
    {
        OnSleepActionStart?.Invoke();
        StartCoroutine(Sleep());
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Controller : CommandRunner, IMouseDrag, ICommandReader, ICommandReciever
{
    public Direction direction;

    [SerializeField] float controlPointSpacing = 2f;
    [SerializeField] float predictionLineLength = 200f;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] LineRenderer predictionLine;
    [SerializeField] GameObject sprite;

    public event Action OnMouseDragBegin;
    public event Action OnMouseDragEnd;
    public event Action OnFinishCommand;
    public event Action OnFinishSecondLevelCommand;
    public event Action<HexCell, string> OnWarning;

    //recorder:
    Direction recorderDirection;
    Quaternion recorderSpriteRotation;

    List<ICommandReciever> recievers;
    public int connectorCommandCounter;
    public int recievedMovingCommandNum = 0;

    void Awake()
    {
        predictionLine.gameObject.SetActive(false);
        sprite.transform.rotation = Quaternion.Euler(Vector3.forward * -30f);
        recievers = new List<ICommandReciever>();
    }

    void UpdatePredictionLine()
    {
        predictionLine.SetPositions(new Vector3[]
        {
            Vector3.zero,
            (cell.GetNeighbor(direction).transform.position - cell.transform.position).normalized * predictionLineLength
        });
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.controller = this;
        cell.reciever = this;

        transform.position = cell.transform.position;

        predictionLine.gameObject.SetActive(true);
        UpdatePredictionLine();
    }

    public void SetIndex(int index_)
    {
        index = index_;
        indexText.text = index.ToString();
        indexText.gameObject.SetActive(true);
    }

    public void StartDragging()
    {
        predictionLine.gameObject.SetActive(false);
        cell.controller = null;
        cell.reciever = null;

        BuildSystem.Instance.SetCurrentTrackingController(this);
        OnMouseDragBegin?.Invoke();
    }

    public void Rotate(int rotateIndex)
    {
        switch (rotateIndex)
        {
            //ccw
            case -1:
                direction = direction.Previous();
                sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
                break;
            //cw
            case 1:
                direction = direction.Next();
                sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
                break;
        }
    }

    void TrackNewReciever(ICommandReciever reciever)
    {
        recievers.Add(reciever);
        reciever.OnFinishSecondLevelCommand += OnConnectorFinishCommand;
    }

    public void Putdown()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunPutDownUp(true);
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

    public void Putup()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunPutDownUp(false);
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

    public void ConnectorCWRotate()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunRotate(1);
                }
                next = next.GetNeighbor(direction);
            }
            else
            {
                break;
            }
        }

        if(recievers.Count == 0)
        {
            StartCoroutine(Sleep());
        }
    }

    private void OnConnectorFinishCommand()
    {
        print("OnConnectorFinishCommand");
        connectorCommandCounter++;
        if(connectorCommandCounter == recievers.Count)
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

    public void ConnectorCCWRotate()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunRotate(-1);
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

    public void Connect()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunConnect();
                }
                next = next.GetNeighbor(direction);
            }
            else
            {
                break;
            }
        }
    }

    public void Split()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunSplit();
                }
                next = next.GetNeighbor(direction);
            }
            else
            {
                break;
            }
        }
    }

    public void Delay()
    {
        StartCoroutine(Sleep());
    }

    public void Push()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunMove(direction);
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

    public void Pull()
    {
        HexCell next = cell.GetNeighbor(direction);
        for (int i = 0; i < 25; i++)
        {
            if (next != null)
            {
                if (next.reciever != null)
                {
                    TrackNewReciever(next.reciever);
                    next.reciever.RunMove(direction.Opposite());
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
        cell.controller = null;
        cell.reciever = null;
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

        cell.controller = this;
        cell.reciever = this;
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
            OnWarning?.Invoke(cell, "Error#03\nTwo move commands received at the same time!");
            yield return null;
        }

        HexCell target = cell.GetNeighbor(direction);
        cell.controller = null;
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
        if (target.IsEmpty())
        {
            if (!target.beColoring)
            {
                cell = target;
                cell.controller = this;
                cell.reciever = this;
                recievedMovingCommandNum = 0;
            }
            else
            {
                OnWarning?.Invoke(target, "Error#01!\nEntered the unit that has been colored!");
            }
        }
        else
        {
            OnWarning?.Invoke(target, "Error#00!\nTwo devices enter one unit at the same time!");
        }

        OnFinishSecondLevelCommand?.Invoke();
        if(recievers.Count == 0)
        {
            recievedMovingCommandNum = 0;
            connectorCommandCounter = 0;
            OnFinishCommand?.Invoke();
        }
    }

    public void ControllerCCWRotate()
    {
        direction = direction.Previous();
        sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles + Vector3.forward * 60f);
        UpdatePredictionLine();
        StartCoroutine(Sleep());
    }

    public void ControllerCWRotate()
    {
        direction = direction.Next();
        sprite.transform.rotation = Quaternion.Euler(sprite.transform.eulerAngles - Vector3.forward * 60f);
        UpdatePredictionLine();
        StartCoroutine(Sleep());
    }

    public void RunPutDownUp(bool coloring)
    {
        OnFinishCommand?.Invoke();
    }

    public void RunRotate(int rotateDirection)
    {
        recievedMovingCommandNum++;
        OnFinishCommand?.Invoke();
    }

    public void RunMove(Direction moveDirection)
    {
        recievedMovingCommandNum++;
        StartCoroutine(MoveToTarget(moveDirection));
    }

    public void RunConnect()
    {
        OnFinishCommand?.Invoke();
    }

    public void RunSplit()
    {
        OnFinishCommand?.Invoke();
    }
}

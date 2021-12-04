using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Connector : MonoBehaviour, IMouseDrag, ICommandReciever
{
    public List<Brush> brushes;
    public HexCell cell;
    [SerializeField] float controlPointSpacing = 1.5f;
    [SerializeField] float radius = 5f;

    //public event Action OnMouseDragBegin;
    //public event Action OnMouseDragEnd;
    public event Action<Direction> OnMoveActionStart;
    public event Action<int> OnRotateActionStart;
    public event Action OnFinishSecondLevelCommand;

    public int brushFinishCounter;

    public bool HasBeenSetup { get; private set; } = false;

    //recorder:
    HexCell recorderCell;

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.connector = this;
        cell.reciever = this;

        transform.position = cell.transform.position;
        transform.Find("sprite").localScale = radius * Vector3.one;
        brushes = new List<Brush>();
        HasBeenSetup = true;
    }

    public void EnterNewCell(HexCell cell_)
    {
        cell = cell_;
        cell.connector = this;
        cell.reciever = this;
        transform.position = cell.transform.position;
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
        OnFinishSecondLevelCommand?.Invoke();
    }

    IEnumerator MoveToTarget(Direction direction)
    {
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

        cell = target;
        cell.connector = this;
        cell.reciever = this;

        if(brushes.Count == 0)
        {
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    public void Record()
    {
        recorderCell = cell;
    }

    public void Read()
    {
        cell.connector = null;
        cell.reciever = null;

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
    }

    public void RunPutDownUp(bool coloring)
    {
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].PutDownUp(coloring);
        }
    }

    public void RunConnect()
    {
        for (int i = 0; i < cell.neighbors.Length; i++)
        {
            if(cell.neighbors[i] != null)
            {
                if(cell.neighbors[i].brush != null)
                {
                    Brush newBrush = cell.neighbors[i].brush;
                    brushes.Add(newBrush);
                    newBrush.ConnectWithConnector(this);
                    newBrush.OnFinishThirdLevelCommand += OnBrushFinishCommand;
                }
            }
        }
        if (brushes.Count == 0)
        {
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    private void OnBrushFinishCommand()
    {
        brushFinishCounter++;
        if(brushFinishCounter == brushes.Count)
        {
            brushFinishCounter = 0;
            OnFinishSecondLevelCommand?.Invoke();
        }
    }

    public void RunSplit()
    {
        for (int i = 0; i < brushes.Count; i++)
        {
            if(brushes[i] != null)
            {
                brushes[i].OnFinishThirdLevelCommand -= OnBrushFinishCommand;
                brushes[i].SplitWithConnector(this);
                brushes[i] = null;
                brushes.RemoveAt(i);
                i--;
            }
        }
        //instead of animation
        StartCoroutine(Sleep());
    }

    public void RunRotate(int rotateDirection)
    {
        if(brushes.Count == 0)
        {
            StartCoroutine(Sleep());
            return;
        }

        OnRotateActionStart?.Invoke(rotateDirection);
    }

    public void RunMove(Direction moveDirection)
    {
        OnMoveActionStart?.Invoke(moveDirection);
        StartCoroutine(MoveToTarget(moveDirection));
    }
}

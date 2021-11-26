using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ConnecterBrushSlot : CommandReader, IMouseDrag, ICommandReaderObj,  IControlPointRotateObj
{
    public Brush brush;
    public Connecter connecter;
    public HexCell cell;
    public bool coloring;

    [SerializeField] float controlPointSpacing = 2f;
    [SerializeField] ControlPoint controlPoint;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] Direction direction;

    public event Action OnFinishCommand;
    bool movingByConnecter = false;
    public Queue<Direction> moveList;

    //recorder:
    Brush recorderBrush;
    Connecter recorderConnecter;
    HexCell recorderCell;
    Direction recorderDirection;

    void Awake()
    {
        indexText.gameObject.SetActive(false);
        controlPoint.gameObject.SetActive(false);

        moveList = new Queue<Direction>();
        readerLevel = CommandReaderLevel.Child;
    }

    public void Setup(Connecter connecter_, HexCell cell_, Direction direction_)
    {
        connecter = connecter_;
        cell = cell_;
        cell.connecterSlot = this;
        direction = direction_;
        transform.position = cell.transform.position;
        transform.parent = connecter.slotParent;
        connecter.slots[(int)direction] = this;
        controlPoint.Setup(cell, gameObject, this);
        SetControlPointPosition();
        controlPoint.gameObject.SetActive(true);

        connecter.OnMouseDragBegin += Connecter_OnMouseDragBegin;
        connecter.OnMouseDragEnd += Connecter_OnMouseDragEnd;
        connecter.OnMoveActionStart += Connecter_OnMoveActionStart;
        connecter.OnRotateActionStart += Connecter_OnRotateActionStart;
    }

    private void Connecter_OnRotateActionStart(int index)
    {
        moveList.Enqueue(index == 1 ? direction.MoveDirection(direction.Next()) : direction.MoveDirection(direction.Previous()));
    }

    private void Connecter_OnMoveActionStart(Direction direction)
    {
        moveList.Enqueue(direction);
        movingByConnecter = true;
    }

    private void Connecter_OnMouseDragEnd()
    {
        cell = connecter.cell.GetNeighbor(direction);
        cell.connecterSlot = this;
    }

    private void Connecter_OnMouseDragBegin()
    {
        cell.connecterSlot = null;
        cell = null;
    }

    void SetControlPointPosition()
    {
        Vector3 n = (transform.position - connecter.transform.position).normalized;
        controlPoint.transform.localPosition = n * controlPointSpacing;
    }

    public void SetIndex(int index_)
    {
        index = index_;
        indexText.text = index.ToString();
        indexText.gameObject.SetActive(true);
    }

    public void RotateInEditMode()
    {
        float angle = InputHelper.GetAngleFromMousePositionIn2D(connecter.transform.position);
        Direction newDir = InputHelper.GetHexDirectionFromAngle(angle);

        if (newDir != direction)
        {
            HexCell newCell = connecter.cell.GetNeighbor(newDir);
            if (newCell.CanEnterConnecterOrSlot())
            {
                connecter.slots[(int)direction] = null;
                direction = newDir;
                connecter.slots[(int)direction] = this;

                transform.position = newCell.transform.position;
                cell.connecterSlot = null;
                cell = newCell;
                cell.connecterSlot = this;
                SetControlPointPosition();
            }
        }
    }

    public void StartDragging()
    {
        
    }

    public override void Finish()
    {
        OnFinishCommand?.Invoke();
    }

    IEnumerator Sleep(Action callback)
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        callback?.Invoke();
    }

    IEnumerator MoveToTarget(Direction moveDirection, Action callback)
    {
        HexCell target = cell.GetNeighbor(moveDirection);
        if(brush != null)
        {
            brush.cell.brush = null;
            brush.cell = null;
        }
        cell.connecterSlot = null;

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
        cell.connecterSlot = this;
        direction = connecter.cell.GetHexDirection(cell);

        if (brush != null)
        {
            brush.cell = target;
            target.brush = brush;

            if (coloring)
            {
                cell.Coloring(brush.colorSO.color);
            }
        }

        callback?.Invoke();
    }

    void OnceAction()
    {
        if(moveList.Count != 0)
        {
            Direction direction = moveList.Dequeue();
            StartCoroutine(MoveToTarget(direction, OnceAction));
        }
        else
        {
            movingByConnecter = false;
            OnFinishCommand?.Invoke();
        }
    }

    public void Drop()
    {
        coloring = true;

        if (brush != null)
        {
            cell.Coloring(brush.colorSO.color);
        }
        OnceAction();
    }

    public void Pick()
    {
        coloring = false;
        OnceAction();
    }

    public void ClockwiseRotate()
    {
        if(moveList.Count == 0)
        {
            moveList.Enqueue(direction.MoveDirection(direction.Next()));
        }
        else
        {
            if (movingByConnecter)
            {
                moveList.Enqueue(direction.MoveDirection(direction.Next()));
            }
            else
            {
                Direction middle = direction.MoveByDirection(moveList.Peek());
                moveList.Enqueue(middle.MoveDirection(middle.Next()));
            }
        }
        OnceAction();
    }

    public void CounterClockwiseRotate()
    {
        if (moveList.Count == 0)
        {
            moveList.Enqueue(direction.MoveDirection(direction.Previous()));
        }
        else
        {
            if (movingByConnecter)
            {
                moveList.Enqueue(direction.MoveDirection(direction.Previous()));
            }
            else
            {
                Direction middle = direction.MoveByDirection(moveList.Peek());
                moveList.Enqueue(middle.MoveDirection(middle.Previous()));
            }
        }
        OnceAction();
    }

    public void Connect()
    {
        if (cell.brush != null)
        {
            brush = cell.brush;
            brush.transform.parent = transform;
            brush.slot = this;
        }
        OnceAction();
    }

    public void Split()
    {
        if(brush != null)
        {
            brush.transform.parent = null;
            brush.slot = null;
            brush = null;
        }
        OnceAction();
    }

    public void Delay()
    {
        OnceAction();
    }

    public void GoAhead() { print("WRONG COMMAND: Child can't go ahead!"); }

    public void Back() { print("WRONG COMMAND: Child can't go back!"); }

    public override void Record()
    {
        recorderDirection = direction;
        recorderBrush = brush;
        recorderCell = cell;
        recorderConnecter = connecter;

        controlPoint.gameObject.SetActive(false);
    }

    public override void Read()
    {
        moveList.Clear();
        cell.connecterSlot = null;

        brush = recorderBrush;
        cell = recorderCell;
        direction = recorderDirection;
        connecter = recorderConnecter;
        transform.position = cell.transform.position;

        controlPoint.gameObject.SetActive(true);
        SetControlPointPosition();

        cell.connecterSlot = this;
        recorderBrush = null;
        recorderCell = null;
        recorderConnecter = null;
        brush = null;
    }
}

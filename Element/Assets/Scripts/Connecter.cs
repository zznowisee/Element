using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Connecter : CommandReader, IMouseDrag, IControlPointRotateObj, ICommandReaderObj
{
    public HexCell cell;
    public ConnecterBrushSlot[] slots;
    public Direction direction;

    [SerializeField] float controlPointSpacing = 1.5f;
    [SerializeField] float radius = 5f;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] Transform directionPoint;
    [SerializeField] ControlPoint controlPoint;
    public Transform slotParent;

    public event Action OnMouseDragBegin;
    public event Action OnMouseDragEnd;
    public event Action<Direction> OnMoveActionStart;
    public event Action<int> OnRotateActionStart;
    public event Action OnFinishCommand;
    public bool HasBeenSetup { get; private set; } = false;

    //recorder:
    HexCell recorderCell;
    Direction recorderDirection;

    private void Awake()
    {
        indexText.gameObject.SetActive(false);
        directionPoint.gameObject.SetActive(false);
        controlPoint.gameObject.SetActive(false);
        direction = Direction.NE;
        slots = new ConnecterBrushSlot[6];

        readerLevel = CommandReaderLevel.Parent;
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.connecter = this;
        transform.position = cell.transform.position;
        SetDirectionPointPosition();
        transform.Find("sprite").localScale = radius * Vector3.one;
        directionPoint.gameObject.SetActive(true);
        controlPoint.gameObject.SetActive(true);
        controlPoint.Setup(cell, gameObject, this);

        HasBeenSetup = true;
    }

    public void EnterNewCell(HexCell cell_)
    {
        cell = cell_;
        cell.connecter = this;
        transform.position = cell.transform.position;

        OnMouseDragEnd?.Invoke();
    }

    public void SetIndex(int index_)
    {
        index = index_;
        indexText.text = index.ToString();
        indexText.gameObject.SetActive(true);
    }

    public void StartDragging()
    {
        cell.connecter = null;
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingConnecter(this);
        OnMouseDragBegin?.Invoke();
    }

    void SetDirectionPointPosition()
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        Vector3 n = (neighbor.transform.position - transform.position).normalized;
        directionPoint.transform.localPosition = n * radius / 2f;
        controlPoint.transform.localPosition = n * (radius / 2f + controlPointSpacing);
    }

    public void RotateInEditMode()
    {
        float angle = InputHelper.GetAngleFromMousePositionIn2D(transform.position);
        direction = InputHelper.GetHexDirectionFromAngle(angle);
        SetDirectionPointPosition();
    }

    public void Drop()
    {
        print($"{index}_Connecter Run <Drop>");
    }

    public void Pick()
    {
        print($"{index}_Connecter Run <Pick>");
    }

    public void ClockwiseRotate()
    {
        print($"{index}_Connecter Run <ClockwiseRotate>");
        OnRotateActionStart?.Invoke(1);
        direction = direction.Next();
        SetDirectionPointPosition();

        //replace by rotate animation
        StartCoroutine(Sleep(Finish));
    }

    public void CounterClockwiseRotate()
    {
        print($"{index}_Connecter Run <CounterClockwiseRotate>");
        OnRotateActionStart?.Invoke(-1);
        direction = direction.Previous();
        SetDirectionPointPosition();

        //replace by rotate animation
        StartCoroutine(Sleep(Finish));
    }

    public void Connect()
    {
        print($"{index}_Connecter Run <Connect>");
    }

    public void Split()
    {
        print($"{index}_Connecter Run <Split>");
    }

    public void Delay()
    {
        print($"{index}_Connecter Run <Delay>");
        StartCoroutine(Sleep(Finish));
    }

    public void GoAhead()
    {
        print($"{index}_Connecter Run <GoAhead>");
        OnMoveActionStart?.Invoke(direction);
        StartCoroutine(MoveToTarget(direction, Finish));
    }

    public void Back()
    {
        print($"{index}_Connecter Run <Back>");
        OnMoveActionStart?.Invoke(direction.Opposite());
        StartCoroutine(MoveToTarget(direction.Opposite(), Finish));
    }

    IEnumerator MoveToTarget(Direction direction, Action callback)
    {
        HexCell target = cell.GetNeighbor(direction);
        cell.connecter = null;
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
        cell.connecter = this;
        callback?.Invoke();
    }

    IEnumerator Sleep(Action callback)
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        callback?.Invoke();
    }

    public override void Finish()
    {
        OnFinishCommand?.Invoke();
    }

    public override void Record()
    {
        controlPoint.gameObject.SetActive(false);
        recorderCell = cell;
        recorderDirection = direction;
    }

    public override void Read()
    {
        cell.connecter = null;
        cell = recorderCell;
        direction = recorderDirection;
        cell.connecter = this;

        transform.position = cell.transform.position;
        recorderCell = null;

        SetDirectionPointPosition();
        controlPoint.gameObject.SetActive(true);
    }
}

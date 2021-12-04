using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brush : MonoBehaviour, IMouseDrag
{
    public HexCell cell;
    public HexCell recorderCell;
    public Connector connector;
    public ColorSO colorSO;
    public bool putdown;

    [SerializeField] protected LineRenderer connectLine;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;

    public event Action OnFinishThirdLevelCommand;

    public void StartDragging()
    {
        cell.brush = null;
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingBrush(this);
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.brush = this;
        transform.position = cell.transform.position;
    }

    public void SetColorSO(ColorSO color_)
    {
        colorSO = color_;
        meshRenderer.material.color = colorSO.color;
    }

    public void ClearColorSO()
    {
        colorSO = null;
    }

    public void Record()
    {
        recorderCell = cell;
    }

    public void Read()
    {
        cell.brush = null;

        cell = recorderCell;
        cell.brush = this;

        recorderCell = null;
        transform.position = cell.transform.position;

        connectLine.SetPosition(0, Vector3.zero);
        connectLine.SetPosition(1, Vector3.zero);
        connectLine.gameObject.SetActive(false);
        if (connector != null)
        {
            connector.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector.OnRotateActionStart -= Connector_OnRotateActionStart;
            connector = null;
        }

        putDownSprite.SetActive(false);
        putdown = false;
    }

    public void ConnectWithConnector(Connector connector_)
    {
        if (connector == null)
        {
            connector = connector_;
            connector.OnMoveActionStart += Connector_OnMoveActionStart;
            connector.OnRotateActionStart += Connector_OnRotateActionStart;
            connectLine.gameObject.SetActive(true);
            connectLine.SetPosition(0, cell.transform.position - transform.position);
            connectLine.SetPosition(1, connector.transform.position - transform.position);
        }

        StartCoroutine(Sleep());
    }

    public void SplitWithConnector(Connector connector_)
    {
        if (connector == connector_)
        {
            connector.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector.OnRotateActionStart -= Connector_OnRotateActionStart;
            connectLine.gameObject.SetActive(false);
            connectLine.SetPosition(0, Vector3.zero);
            connectLine.SetPosition(1, Vector3.zero);
            connector = null;
        }

        StartCoroutine(Sleep());
    }

    public void Connector_OnRotateActionStart(int rotateIndex)
    {
        HexCell target;
        switch (rotateIndex)
        {
            //ccw
            case -1:
                //from connector to this 's cell direction
                target = connector.cell.PreviousCell(cell);
                StartCoroutine(MoveToTarget(target, OnFinishThirdLevelCommand));
                break;
            //cw
            case 1:
                target = connector.cell.NextCell(cell);
                StartCoroutine(MoveToTarget(target, OnFinishThirdLevelCommand));
                break;
        }
    }

    public virtual void PutDownUp(bool putdown_)
    {
        putdown = putdown_;
        putDownSprite.SetActive(putdown);
        StartCoroutine(Sleep());
    }

    public void Connector_OnMoveActionStart(Direction direction)
    {
        StartCoroutine(MoveToTarget(cell.GetNeighbor(direction), OnFinishThirdLevelCommand));
    }

    public virtual IEnumerator MoveToTarget(HexCell target, Action callback)
    {
        yield return null;
    }

    public IEnumerator Sleep()
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        OnFinishThirdLevelCommand?.Invoke();
    }
}

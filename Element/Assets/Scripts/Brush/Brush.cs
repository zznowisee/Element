using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brush : MonoBehaviour, IMouseDrag
{
    public HexCell cell;
    HexCell recorderCell;

    [SerializeField] LineRenderer connectLine;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;

    public Connector connector;
    public ColorSO colorSO;
    public bool coloring;

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
        if(connector != null)
        {
            connector.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector.OnRotateActionStart -= Connector_OnRotateActionStart;
            connector = null;
        }

        putDownSprite.SetActive(false);
        coloring = false;
    }

    public void ConnectWithConnector(Connector connector_)
    {
        if(connector == null)
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

    private void Connector_OnRotateActionStart(int rotateIndex)
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

    public void PutDownUp(bool coloring_)
    {
        if (coloring_)
        {
            putDownSprite.SetActive(true);
            coloring = true;
        }
        else
        {
            putDownSprite.SetActive(false);
            coloring = false;
        }
        StartCoroutine(Sleep());
    }

    private void Connector_OnMoveActionStart(Direction direction)
    {
        StartCoroutine(MoveToTarget(cell.GetNeighbor(direction), OnFinishThirdLevelCommand));
    }

    IEnumerator MoveToTarget(HexCell target, Action callback)
    {
        cell.brush = null;
        
        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.SetPosition(1, connector.transform.position - transform.position);
            yield return null;
        }

        cell = target;
        cell.brush = this;
        Coloring();
        callback?.Invoke();
    }

    public void Coloring()
    {
        cell.Coloring(colorSO.color);
    }

    IEnumerator Sleep()
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        OnFinishThirdLevelCommand?.Invoke();
    }
}
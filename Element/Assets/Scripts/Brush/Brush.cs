using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brush : MonoBehaviour, IMouseDrag
{
    [HideInInspector] public HexCell cell;
    [HideInInspector] public HexCell recorderCell;
    [HideInInspector] public ColorSO colorSO;
    public bool putdown;
    public LineRenderer connectLine;
    [SerializeField] protected LineRenderer pfConnectLine;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;
    public Connector connector;
    public event Action OnFinishThirdLevelCommand;
    public virtual event Action<Vector3, WarningType> OnWarning;

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

    public void ClearCurrentInfo()
    {
        if(connector != null)
        {
            connector.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector.OnRotateActionStart -= Connector_OnRotateActionStart;
            connector.OnSleepActionStart -= Connector_OnSleepActionStart;
            connector = null;
        }

        if(connectLine != null)
        {
            Destroy(connectLine.gameObject);
            connectLine = null;
        }

        cell.brush = null;
    }

    public void ReadPreviousInfo()
    {
        cell = recorderCell;
        cell.brush = this;

        recorderCell = null;
        transform.position = cell.transform.position;
        putDownSprite.SetActive(false);
        putdown = false;
    }

    public virtual void ConnectWithConnector(Connector connector_) { }

    public void Connector_OnSleepActionStart()
    {
        StartCoroutine(Sleep());
    }

    public void SplitWithConnector(Connector connector_)
    {
        if (connector == connector_)
        {
            connector_.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector_.OnRotateActionStart -= Connector_OnRotateActionStart;
            connector_.OnSleepActionStart -= Connector_OnSleepActionStart;
            Destroy(connectLine.gameObject);
            connectLine = null;
            connector = null;
        }
    }

    public void Connector_OnRotateActionStart(Connector connector, int rotateIndex)
    {
        switch (rotateIndex)
        {
            //ccw
            case -1:
                //from connector to this 's cell direction
                StartCoroutine(MoveToTarget(connector, connector.cell.PreviousCell(cell), OnFinishThirdLevelCommand));
                break;
            //cw
            case 1:
                StartCoroutine(MoveToTarget(connector, connector.cell.NextCell(cell), OnFinishThirdLevelCommand));
                break;
        }
    }

    public virtual void PutDownUp(bool putdown_)
    {
        putdown = putdown_;
        putDownSprite.SetActive(putdown);
        StartCoroutine(Sleep());
    }

    public void Connector_OnMoveActionStart(Connector connector, Direction direction)
    {
        StartCoroutine(MoveToTarget(connector, cell.GetNeighbor(direction), OnFinishThirdLevelCommand));
    }

    public virtual IEnumerator MoveToTarget(Connector connector, HexCell target, Action callback)
    {
        return null;
    }

    public IEnumerator Sleep()
    {
        yield return new WaitForSeconds(ProcessSystem.Instance.commandDurationTime);
        OnFinishThirdLevelCommand?.Invoke();
    }
}

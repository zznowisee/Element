using System;
using System.Collections;
using UnityEngine;
public enum BrushType
{
    Coloring,
    Line
}
public class Brush : MonoBehaviour, IMouseDrag
{

    public BrushData brushData;
    [HideInInspector] public BrushBtn brushBtn;
    [HideInInspector] public HexCell cell;
    [HideInInspector] public HexCell recorderCell;
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
        cell.currentObject = null;
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingBrush(this);
    }

    public void OnSwitchToMainScene()
    {
        cell.currentObject = null;
        Destroy(gameObject);
    }

    public void SetupFromBtn(BrushBtn brushBtn_)
    {
        brushBtn = brushBtn_;
        brushData.colorSO = brushBtn.colorSO;
        brushData.type = brushBtn.brushType;
        brushBtn.brushDatas.Add(brushData);

        meshRenderer.material.color = brushData.colorSO.drawColor;
    }

    public void SetupFromData(HexCell cell_, BrushData data_, BrushBtn brushBtn_)
    {
        brushData = data_;
        brushBtn = brushBtn_;
        Setup(cell_);

        meshRenderer.material.color = brushData.colorSO.drawColor;
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;
        brushData.cellIndex = cell.index;
    }

    public void Record()
    {
        recorderCell = cell;
    }

    public void ClearCurrentInfo()
    {
        if (connector != null)
        {
            connector.OnMoveActionStart -= Connector_OnMoveActionStart;
            connector.OnRotateActionStart -= Connector_OnRotateActionStart;
            connector.OnSleepActionStart -= Connector_OnSleepActionStart;
            connector = null;
        }

        if (connectLine != null)
        {
            Destroy(connectLine.gameObject);
            connectLine = null;
        }

        cell.currentObject = null;
    }

    public void ReadPreviousInfo()
    {
        cell = recorderCell;
        cell.currentObject = gameObject;

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

    public void Connector_OnRotateActionStart(Connector connector, RotateDirection rotateDirection)
    {
        switch (rotateDirection)
        {
            //ccw
            case RotateDirection.CounterClockwise:
                //from connector to this 's cell direction
                StartCoroutine(MoveToTarget(connector, connector.cell.PreviousCell(cell), OnFinishThirdLevelCommand));
                break;
            //cw
            case RotateDirection.Clockwise:
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

    public void OnDisable()
    {
        brushBtn.OnDestroyBrush(brushData);
    }
}

using System;
using System.Collections;
using UnityEngine;

[Serializable]
public enum BrushType
{
    Coloring,
    Line
}

public class Brush : ConnectableDevice, IMouseAction
{

    public BrushData brushData;
    [HideInInspector] public BrushBtn brushBtn;
    [HideInInspector] public HexCell recorderCell;
    public bool putdown;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;
    public Connector connector;
    public virtual event Action<Vector3, WarningType> OnWarning;

    void Start()
    {
        putDownSprite.gameObject.SetActive(false);
    }

    public void MouseAction_Drag()
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
        brushData.colorType = brushBtn.colorSO.colorType;
        brushData.type = brushBtn.brushType;
        brushBtn.brushDatas.Add(brushData);
        meshRenderer.material.color = brushBtn.colorSO.drawColor;
    }

    public void SetupFromData(HexCell cell_, BrushData data_, ColorSO colorSO_, BrushBtn brushBtn_)
    {
        brushData = data_;
        brushBtn = brushBtn_;
        Setup(cell_);
        meshRenderer.material.color = colorSO_.drawColor;
    }

    public void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;
        brushData.cellIndex = cell.index;
    }

    public override void Record()
    {
        recorderCell = cell;
    }

    public override void ClearCurrentInfo()
    {
        
        if (connectLine != null)
        {
            Destroy(connectLine.gameObject);
            connectLine = null;
        }

        cell.currentObject = null;
    }

    public override void ReadPreviousInfo()
    {
        cell = recorderCell;
        cell.currentObject = gameObject;

        recorderCell = null;
        transform.position = cell.transform.position;
        putDownSprite.SetActive(false);
        putdown = false;
    }

    public virtual void ConnectWithConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_) { }

    public void Connector_OnRotateActionStart(Action callback, Action<Action> secondLevelCallback, Connector connector, RotateDirection rotateDirection)
    {
        switch (rotateDirection)
        {
            //ccw
            case RotateDirection.CCW:
                //from connector to this 's cell direction
                //StartCoroutine(MoveToTarget(callback, secondLevelCallback, connector, connector.cell.PreviousCell(cell)));
                break;
            //cw
            case RotateDirection.CW:
                //StartCoroutine(MoveToTarget(callback, secondLevelCallback, connector, connector.cell.NextCell(cell)));
                break;
        }
    }

    public virtual void PutDownUp(bool putdown_)
    {
        putdown = putdown_;
        putDownSprite.SetActive(putdown);
    }

    public void OnDestroyByPlayer()
    {
        brushBtn.OnDestroyBrush(brushData);
    }

    public override void ConnectWithConnector(ConnectableDevice target)
    {
        base.ConnectWithConnector(target);
    }

    public override void SplitWithConnector(ConnectableDevice target)
    {
        base.SplitWithConnector(target);
    }
}

using System;
using UnityEngine;

[Serializable]
public enum BrushType
{
    Coloring,
    Line
}

public class Brush : ConnectableDevice, ISelectable
{
    public BrushData data;

    [HideInInspector] public HexCell recorderCell;
    public BrushState brushState;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;
    public event Action<Brush> OnDestoryByPlayer;
    public Color drawCol;

    void Awake()
    {
        deviceType = DeviceType.Brush;    
    }

    public virtual void Start()
    {
        putDownSprite.gameObject.SetActive(false);
        deviceType = DeviceType.Brush;
    }

    void UpdateColor()
    {
        drawCol = ColorManager.Instance.GetColorSOFromColorType(data.colorType).drawColor;
        meshRenderer.material.color = drawCol;
    }

    public virtual void Rebuild(HexCell cell_, BrushData data_)
    {
        data = data_;
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;

        UpdateColor();
    }

    public void Init(BrushType brushType, ColorType colorType)
    {
        data.brushType = brushType;
        data.colorType = colorType;

        UpdateColor();
    }

    public override void Setup(HexCell cell_)
    {
        base.Setup(cell_);
        data.cellIndex = cell.index;
        data.deviceType = DeviceType.Brush;
        brushState = BrushState.PUTUP;
    }

    public override void Record()
    {
        base.Record();
    }

    public override void ClearCurrentInfo()
    {
        base.ClearCurrentInfo();

        connectingConnector = null;
        if (connectLine != null)
        {
            Destroy(connectLine.gameObject);
            connectLine = null;
        }
    }

    public override void ReadPreviousInfo()
    {
        base.ReadPreviousInfo();

        putDownSprite.SetActive(false);
        brushState = BrushState.PUTUP;
    }

    public override void PutDownUp(Action releaserCallback, Action<Action> recieverCallback, BrushState state, float executeTime)
    {
        brushState = state;
        putDownSprite.gameObject.SetActive(state == BrushState.PUTDOWN);
    }

    public override void ConnectWithConnector(Connector target)
    {
        base.ConnectWithConnector(target);
    }

    public override void SplitWithConnector(Connector target)
    {
        base.SplitWithConnector(target);
    }

    public override void DestoryDevice()
    {
        OnDestoryByPlayer?.Invoke(this);
        base.DestoryDevice();
    }
}

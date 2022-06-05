using System;
using UnityEngine;

public class Device_Brush : ConnectableDevice
{
    public Data_Brush data;
    public BrushType brushType;
    [HideInInspector] public HexCell recorderCell;
    public BrushState brushState;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject putDownSprite;
    public event Action<Device_Brush> OnDestory;
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
        drawCol = ColorManager.Instance.GetColor(data.colorType, false);
        meshRenderer.material.color = drawCol;
    }

    public virtual void Rebuild(HexCell cell_, Data_Brush data_)
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
        data.index = cell.hexCoordinates.GetValue();
        //data.cellIndex = cell.index;
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


    public override void Putdown(Action releaserCallback, Action<Action> recieverCallback, float executeTime)
    {
        brushState = BrushState.PUTDOWN;
        putDownSprite.gameObject.SetActive(true);
    }

    public override void Putup(Action releaserCallback, Action<Action> recieverCallback, float executeTime)
    {
        brushState = BrushState.PUTUP;
        putDownSprite.gameObject.SetActive(false);
    }

    public override void ConnectWithConnector(Device_Connector target)
    {
        beConnectedThisFrame = true;

        base.ConnectWithConnector(target);
    }

    public override void SplitWithConnector(Device_Connector target)
    {
        base.SplitWithConnector(target);
    }

    protected override void DestoryByPlayer()
    {
        OnDestory?.Invoke(this);
        base.DestoryByPlayer();
    }
}

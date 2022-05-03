using UnityEngine;
using System;

public enum DeviceType
{
    Connector,
    Controller,
    Brush
}

public class Device : MonoBehaviour, ISelectable
{
    [HideInInspector] public DeviceType deviceType;
    [HideInInspector] public int index;
    [HideInInspector] public HexCell cell;
    [HideInInspector] public HexCell recordCell;

    public SelectableType type { get; set; }
    public bool selected { get; set; }
    public bool preSelected { get; set; }
    public Vector3 delta { get; set; }
    public virtual void Record()
    {
        recordCell = cell;
    }

    public virtual void ClearCurrentInfo()
    {
        cell.currentObject = null;
    }

    public virtual void ReadPreviousInfo()
    {
        cell = recordCell;
        cell.currentObject = gameObject;
        recordCell = null;
        transform.position = cell.transform.position;
    }

    public virtual void DestoryDevice()
    {
        if (cell != null)
        {
            cell.currentObject = null;
        }
        Destroy(gameObject);
    }

    public virtual void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;
    }

    public virtual void OnEnable()
    {
        MouseManager.Instance.AddISelectableObj(this);

        ProcessManager.Instance.OnRecord += Record;
        ProcessManager.Instance.OnClear += ClearCurrentInfo;
        ProcessManager.Instance.OnRead += ReadPreviousInfo;
        ProcessManager.Instance.OnTriggerBUG += StopAllCoroutines;

        OperatingRoomUI.Instance.OnSwitchToMainScene += SwitchToMainScene;
        OperatingRoomUI.Instance.OnStop += StopAllCoroutines;
    }
    public virtual void OnDisable()
    {
        MouseManager.Instance.RemoveISelectableObj(this);

        ProcessManager.Instance.OnRecord -= Record;
        ProcessManager.Instance.OnClear -= ClearCurrentInfo;
        ProcessManager.Instance.OnRead -= ReadPreviousInfo;
        ProcessManager.Instance.OnTriggerBUG -= TriggerBUG;

        OperatingRoomUI.Instance.OnSwitchToMainScene -= SwitchToMainScene;
        OperatingRoomUI.Instance.OnStop -= StopAllCoroutines;
    }

    public virtual void SwitchToMainScene()
    {
        if(cell != null)
        {
            cell.currentObject = null;
        }
        Destroy(gameObject);
    }

    public virtual void TriggerBUG()
    {
        StopAllCoroutines();
    }


    public virtual void LeftClick()
    {
        if (cell != null)
        {
            cell.currentObject = null;
            cell = null;
        }
    }

    public virtual void RightClick()
    {
        DestoryDevice();
    }

    public virtual void Dragging()
    {
        transform.position = InputHelper.MouseWorldPositionIn2D;
    }

    public virtual void LeftRelease(out bool destory)
    {
        if (InputHelper.IsPositionOverUIObject(transform.position))
        {
            print($"{gameObject.name} Object over UI object, Destory");
            DestoryDevice();
            destory = true;
        }
        else
        {
            HexCell cell = InputHelper.GetHexCellUnderPosition3D(transform.position);
            if(cell != null)
            {
                if (cell.IsEmpty())
                {
                    Setup(cell);
                    destory = false;
                }
                else
                {
                    DestoryDevice();
                    destory = true;
                }
            }
            destory = true;
        }
    }

    public void DraggingWithList(Vector3 position)
    {
        transform.position = position;
    }
}

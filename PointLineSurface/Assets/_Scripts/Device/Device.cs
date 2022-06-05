using UnityEngine;
using System;

public class Device : MonoBehaviour, ISelectable
{
    [HideInInspector] public DeviceType deviceType;
    public HexCell cell;
    public HexCell recordCell;

    public event Action<Device> OnDestory;
    public event Action<Device> OnDestoryByPlayer;

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

    public virtual void Setup(HexCell cell_)
    {
        cell = cell_;
        cell.currentObject = gameObject;
        transform.position = cell.transform.position;
    }

    public virtual void OnEnable()
    {
        UI_MouseManager.Instance.AddISelectableObj(this);

        ProcessManager.Instance.OnRecord += Record;
        ProcessManager.Instance.OnClear += ClearCurrentInfo;
        ProcessManager.Instance.OnRead += ReadPreviousInfo;
        UI_WarningManager.Instance.OnWarning += StopAllCoroutines;

        UI_OperateScene.Instance.OnExitOperateScene += DestoryBySwitchScene;
        UI_OperateScene.Instance.OnStop += StopAllCoroutines;
    }
    public virtual void OnDisable()
    {
        // Mouse input
        UI_MouseManager.Instance.RemoveISelectableObj(this);
        // Process 
        ProcessManager.Instance.OnRecord -= Record;
        ProcessManager.Instance.OnClear -= ClearCurrentInfo;
        ProcessManager.Instance.OnRead -= ReadPreviousInfo;
        UI_WarningManager.Instance.OnWarning -= TriggerBUG;

        UI_OperateScene.Instance.OnExitOperateScene -= DestoryBySwitchScene;
        UI_OperateScene.Instance.OnStop -= StopAllCoroutines;
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

    public virtual void RightClick() => DestoryByPlayer();

    public virtual void Dragging()
    {
        transform.position = InputHelper.MouseWorldPositionIn2D;
    }

    public virtual void LeftRelease(out bool destory)
    {
        if (InputHelper.IsPositionOverUIObject(transform.position))
        {
            print($"{gameObject.name} Object over UI object, Destory");
            DestoryByPlayer();
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
                    DestoryByPlayer();
                    destory = true;
                }
            }
            else
            {
                destory = true;
                DestoryByPlayer();
            }
            destory = true;
        }
    }

    public void DraggingWithList(Vector3 position) => transform.position = position;

    protected virtual void DestoryBySwitchScene()
    {
        OnDestory?.Invoke(this);

        if (cell != null) cell.currentObject = null;
        Destroy(gameObject);
    }

    protected virtual void DestoryByPlayer()
    {
        OnDestory?.Invoke(this);
        OnDestoryByPlayer?.Invoke(this);

        if (cell != null) cell.currentObject = null;
        Destroy(gameObject);
        DataManager.Instance.RemoveDeviceData(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildSystem : MonoBehaviour
{
    public event Action<ControlPoint> OnCreateNewControlPoint;

    public static BuildSystem Instance { get; private set; }

    [SerializeField] Wire pfWire;
    Wire trackingWire;
    HexCell trackingCell;

    [SerializeField] FullColorBrush pfFullColorBrush;
    [SerializeField] ColorPicker pfColorPicker;
    [SerializeField] ControlPoint pfControlPoint;
    [SerializeField] Command pfCommand;

    FullColorBrush currentBrush;
    ColorPicker currentColorPicker;
    ControlPoint currentControlPoint;
    Command currentCommand;

    List<int> controlPointIndices;

    void Awake()
    {
        Instance = this;
        controlPointIndices = new List<int>();
    }

    void Update()
    {
        HandleMouseDrag();
        HandleControlPointBuilding();
        HandleColorPickerBuilding();
        HandleBrushBuilding();
        HandleWireBuilding();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && !InputHelper.IsMouseOverUIObject())
        {
            IMouseDrag mouseDrag = InputHelper.GetIMouseDragUnderPosition2D();
            if (mouseDrag != null)
            {
                mouseDrag.StartDragging();
            }
        }

    }

    int GetControlPointIndex()
    {
        for (int i = 0; i < controlPointIndices.Count; i++)
        {
            if (i != controlPointIndices[i])
            {
                controlPointIndices.Insert(i, i);
                return i;
            }
        }

        int index = controlPointIndices.Count;
        controlPointIndices.Add(index);
        return index;
    }

    void HandleControlPointBuilding()
    {
        if(currentControlPoint != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                WayPoint wayPoint = InputHelper.GetWayPointPosition2D();
                if(wayPoint != null)
                {
                    if (wayPoint.IsEmptyForControlPoint())
                    {
                        if (!currentControlPoint.isReady)
                        {
                            OnCreateNewControlPoint?.Invoke(currentControlPoint);
                        }

                        currentControlPoint.Setup(wayPoint);
                        currentControlPoint = null;
                        return;
                    }
                }

                controlPointIndices.Remove(currentControlPoint.index);
                currentControlPoint.Delete();
                currentControlPoint = null;
                return;
            }

            currentControlPoint.transform.position = InputHelper.MouseWorldPositionIn2D;
        }
    }

    void HandleColorPickerBuilding()
    {
        if (currentColorPicker != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                FullColorBrush brush = InputHelper.GetColorBrushUnderPosition2D();
                if (brush != null)
                {
                    brush.SetColorSO(currentColorPicker.color);
                    Destroy(currentColorPicker.gameObject);
                    currentColorPicker = null;
                    return;
                }
                else
                {
                    Destroy(currentColorPicker.gameObject);
                    currentColorPicker = null;
                    return;
                }
            }

            currentColorPicker.transform.position = InputHelper.MouseWorldPositionIn2D;
        }
    }

    void HandleWireBuilding()
    {
        if (InputHelper.IsMouseOverUIObject())
        {
            return;
        }
        if (currentBrush != null || currentColorPicker != null ||
            currentCommand != null || currentControlPoint != null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            InitWire();
        }
        else if (Input.GetMouseButton(0))
        {
            BuildWire();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            FinishWire();
        }
    }

    void HandleBrushBuilding()
    {
        if (currentBrush != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    Destroy(currentBrush.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    currentBrush.Setup(cell);
                    currentBrush = null;
                    return;
                }
            }

            currentBrush.transform.position = InputHelper.MouseWorldPositionIn2D;
        }
    }

    public void CreateNewControlPoint()
    {
        currentControlPoint = Instantiate(pfControlPoint);
        currentControlPoint.SetIndex(GetControlPointIndex());
    }

    public void CreateNewFullColorBrush()
    {
        currentBrush = Instantiate(pfFullColorBrush);
    }

    public void CreateColorPicker(ColorSO color_)
    {
        currentColorPicker = Instantiate(pfColorPicker);
        currentColorPicker.Setup(color_);
    }

    public void SetCurrentTrackingControlPoint(ControlPoint point)
    {
        currentControlPoint = point;
    }

    public void SetCurrentTrackingBrush(FullColorBrush brush)
    {
        currentBrush = brush;
    }

    void InitWire()
    {
        HexCell cell = InputHelper.GetHexCellUnderPosition3D();
        if(cell != null)
        {
            if (cell.CanInitNewWire())
            {
                trackingWire = Instantiate(pfWire, cell.transform.position, Quaternion.identity);
                trackingWire.Init(cell);

                trackingCell = cell;
            }
            else
            {
                WayPoint endOfWire = cell.GetEndOfWireWayPoint();
                if(endOfWire != null)
                {
                    trackingWire = endOfWire.wire;
                    trackingWire.BeSelected(endOfWire, cell);
                    trackingCell = cell;
                }
            }
        }
    }

    bool IsValid(HexCell cell)
    {
        if(trackingCell == null)
        {
            return false;
        }
        return trackingCell.IsNeighbor(cell);
    }

    void BuildWire()
    {
        HexCell cell = InputHelper.GetHexCellUnderPosition3D();
        if(cell != null)
        {
            if(cell != trackingCell)
            {
                if (IsValid(cell))
                {
                    if (trackingWire != null)
                    {
                        if (trackingWire.IsLastCell(cell))
                        {
                            trackingWire.RemovePreviousCell();
                        }
                        else
                        {
                            trackingWire.SetToNewCell(cell);
                        }

                        trackingCell = cell;
                    }
                }
            }
        }
    }

    void FinishWire()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(trackingWire != null)
            {
                trackingWire.Finish();
                trackingWire = null;
                trackingCell = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildSystem : MonoBehaviour
{
    public event Action<Brush> OnCreateNewBrush;
    public event Action<Brush> OnDestoryBrush;

    public event Action<Connecter> OnCreateNewConnecter;
    public event Action<Connecter> OnDestoryConnecter;

    public event Action<ConnecterBrushSlot> OnCreateNewSlot;
    public event Action<ConnecterBrushSlot> OnDestorySlot;

    public static BuildSystem Instance { get; private set; }


    [SerializeField] Track pfTrack;
    [SerializeField] Brush pfColorBrush;
    [SerializeField] ColorPicker pfColorPicker;
    [SerializeField] Connecter pfConnecter;
    [SerializeField] ConnecterBrushSlot pfConnecterBrushSlot;
 
    HexCell currentCell;
    Track currentTrack;
    Brush currentBrush;
    ColorPicker currentColorPicker;
    Connecter currentConnecter;
    ConnecterBrushSlot currentConnecterSlot;
    ControlPoint currentControlPoint;
    List<int> commandReadersIndices;

    void Awake()
    {
        Instance = this;
        commandReadersIndices = new List<int>();
    }

    void Update()
    {
        HandleMouseDrag();
        HandleControlPoint();
        HandleBrushBuilding();
        HandleConnecterBuilding();
        HandleColorPickerBuilding();
        HandleTrackBuilding();
        HandleConnecterSlotBuilding();
    }

    void HandleControlPoint()
    {
        if(Input.GetMouseButtonDown(0) && !InputHelper.IsMouseOverUIObject())
        {
            ControlPoint controlPoint = InputHelper.GetControlPointUnderPosition2D();
            if(controlPoint != null)
            {
                currentControlPoint = controlPoint;
            }
        }

        if (currentControlPoint != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentControlPoint = null;
                return;
            }
            currentControlPoint.controlBody.RotateInEditMode();
        }
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && !InputHelper.IsMouseOverUIObject())
        {
            IMouseDrag mouseDrag = InputHelper.GetIMouseDragUnderPosition2D();
            if (mouseDrag != null)
            {
                mouseDrag.StartDragging();
                return;
            }
        }
    }

    int GetIndexFromIndicesList(List<int> indicesList)
    {
        for (int i = 0; i < indicesList.Count; i++)
        {
            if (i != indicesList[i])
            {
                indicesList.Insert(i, i);
                return i;
            }
        }

        int index = indicesList.Count;
        indicesList.Add(index);
        return index;
    }

    void HandleColorPickerBuilding()
    {
        if (currentColorPicker != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Brush brush = InputHelper.GetColorBrushUnderPosition2D();
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

    void HandleTrackBuilding()
    {
        if(currentTrack != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    Destroy(currentTrack.gameObject);
                    currentTrack = null;
                    return;
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if(cell != null)
                {
                    if (cell.IsEmpty())
                    {
                        cell.SetTrack(currentTrack);
                        currentTrack.Setup(cell);
                        currentTrack = null;
                        return;
                    }
                }

                Destroy(currentTrack.gameObject);
                currentTrack = null;
                return;
            }

            currentTrack.transform.position = InputHelper.MouseWorldPositionIn2D;
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
                    OnDestoryBrush?.Invoke(currentBrush);
                    Destroy(currentBrush.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    if (cell.CanInitNewBrush())
                    {
                        currentBrush.Setup(cell);
                        currentBrush = null;
                        return;
                    }
                }
            }

            currentBrush.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    void HandleConnecterBuilding()
    {
        if (currentConnecter != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    commandReadersIndices.Remove(currentConnecter.index);
                    OnDestoryConnecter?.Invoke(currentConnecter);
                    Destroy(currentConnecter.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    if (cell.CanInitNewBrush())
                    {
                        if (!currentConnecter.HasBeenSetup)
                        {
                            currentConnecter.Setup(cell);
                        }
                        else
                        {
                            currentConnecter.EnterNewCell(cell);
                        }
                        currentConnecter = null;
                        return;
                    }
                }
            }

            currentConnecter.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    void HandleConnecterSlotBuilding()
    {
        if (currentConnecterSlot != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    commandReadersIndices.Remove(currentConnecterSlot.index);
                    OnDestorySlot?.Invoke(currentConnecterSlot);
                    Destroy(currentConnecterSlot.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    Connecter connecter;
                    Direction direction;
                    if (cell.CanInitNewConnecterSlot(out connecter, out direction))
                    {
                        currentConnecterSlot.Setup(connecter, cell, direction);
                        OnCreateNewSlot?.Invoke(currentConnecterSlot);
                        currentConnecterSlot = null;
                        return;
                    }
                    else
                    {
                        commandReadersIndices.Remove(currentConnecterSlot.index);
                        OnDestorySlot?.Invoke(currentConnecterSlot);
                        Destroy(currentConnecterSlot.gameObject);
                    }
                }
            }

            currentConnecterSlot.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    public void CreateNewTrack()
    {
        currentTrack = Instantiate(pfTrack);
    }

    public void CreateNewColorBrush()
    {
        currentBrush = Instantiate(pfColorBrush);
        OnCreateNewBrush?.Invoke(currentBrush);
    }

    public void CreateNewConnecterSlot()
    {
        currentConnecterSlot = Instantiate(pfConnecterBrushSlot);
        int index = GetIndexFromIndicesList(commandReadersIndices);
        currentConnecterSlot.SetIndex(index);
    }

    public void CreateNewConnecter()
    {
        currentConnecter = Instantiate(pfConnecter);
        int index = GetIndexFromIndicesList(commandReadersIndices);
        currentConnecter.SetIndex(index);
        OnCreateNewConnecter?.Invoke(currentConnecter);
    }

    public void CreateColorPicker(ColorSO color_)
    {
        currentColorPicker = Instantiate(pfColorPicker);
        currentColorPicker.Setup(color_);
    }

    public void SetCurrentTrackingBrush(Brush brush)
    {
        currentBrush = brush;
    }

    public void SetCurrentTrackingConnecter(Connecter connecter)
    {
        currentConnecter = connecter;
    }
}

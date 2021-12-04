using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildSystem : MonoBehaviour
{
    public event Action<Brush> OnCreateNewBrush;
    public event Action<Brush> OnDestoryBrush;

    public event Action<Connector> OnCreateNewConnector;
    public event Action<Connector> OnDestoryConnector;

    public event Action<Controller> OnCreateNewController;
    public event Action<Controller> OnDestoryController;

    public static BuildSystem Instance { get; private set; }

    [SerializeField] Track pfTrack;
    [SerializeField] ColorBrush pfColorBrush;
    [SerializeField] LineBrush pfLineBrush;
    [SerializeField] ColorPicker pfColorPicker;
    [SerializeField] Connector pfConnecter;
    [SerializeField] Controller pfController;
 
    HexCell currentCell;
    Track currentTrack;
    Brush currentBrush;
    ColorPicker currentColorPicker;
    Connector currentConnector;
    Controller currentController;
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
        HandleControllerBuilding();
        HandleBrushBuilding();
        HandleConnectorBuilding();
        HandleColorPickerBuilding();
        HandleTrackBuilding();
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

    int GetIndexFromIndicesList()
    {
        for (int i = 0; i < commandReadersIndices.Count; i++)
        {
            if (i != commandReadersIndices[i])
            {
                commandReadersIndices.Insert(i, i);
                return i;
            }
        }

        int index = commandReadersIndices.Count;
        commandReadersIndices.Add(index);
        return index;
    }

    void HandleColorPickerBuilding()
    {
        if (currentColorPicker != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Brush brush = InputHelper.GetBrushUnderPosition2D();
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

    void HandleConnectorBuilding()
    {
        if (currentConnector != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    OnDestoryConnector?.Invoke(currentConnector);
                    Destroy(currentConnector.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    if (cell.CanInitNewBrush())
                    {
                        if (!currentConnector.HasBeenSetup)
                        {
                            currentConnector.Setup(cell);
                        }
                        else
                        {
                            currentConnector.EnterNewCell(cell);
                        }
                        currentConnector = null;
                        return;
                    }
                }
            }

            currentConnector.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    void HandleControllerBuilding()
    {
        if(currentController != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentController.Rotate(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                currentController.Rotate(1);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    commandReadersIndices.Remove(currentController.index);
                    OnDestoryController?.Invoke(currentController);
                    Destroy(currentController.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    if (cell.CanInitNewBrush())
                    {
                        currentController.Setup(cell);
                        currentController = null;
                        return;
                    }
                }
            }

            currentController.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    public void CreateNewTrack()
    {
        currentTrack = Instantiate(pfTrack);
    }

    public void CreateNewController()
    {
        currentController = Instantiate(pfController);
        int index = GetIndexFromIndicesList();
        currentController.SetIndex(index);
        OnCreateNewController?.Invoke(currentController);
    }

    public void CreateNewLineBrush()
    {
        currentBrush = Instantiate(pfLineBrush);
        OnCreateNewBrush?.Invoke(currentBrush);
    }

    public void CreateNewColorBrush()
    {
        currentBrush = Instantiate(pfColorBrush);
        OnCreateNewBrush?.Invoke(currentBrush);
    }

    public void CreateNewConnecter()
    {
        currentConnector = Instantiate(pfConnecter);
        OnCreateNewConnector?.Invoke(currentConnector);
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

    public void SetCurrentTrackingConnector(Connector connector)
    {
        currentConnector = connector;
    }

    public void SetCurrentTrackingController(Controller controller)
    {
        currentController = controller;
    }
}

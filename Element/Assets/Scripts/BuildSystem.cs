using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildSystem : MonoBehaviour
{
    public OperatorDataSO operatorData;
    [SerializeField] HexMap operatorSystem;
    [SerializeField] ProcessSystem processSystem;
    [SerializeField] OperatorUISystem operatorUISystem;
    [SerializeField] MainUISystem mainUISystem;

    public event Action<Brush> OnCreateNewBrush;
    public event Action<Brush> OnDestoryBrush;
    public event Action<Connector> OnCreateNewConnector;
    public event Action<Connector> OnDestoryConnector;

    public event Action<Controller> OnCreateNewController;
    public event Action<Controller> OnDestoryController;

    public static BuildSystem Instance { get; private set; }

    [SerializeField] ColorBrush pfColorBrush;
    [SerializeField] LineBrush pfLineBrush;
    [SerializeField] ColorPicker pfColorPicker;
    [SerializeField] Connector pfConnector;
    [SerializeField] Controller pfController;
 
    Brush currentBrush;
    Connector currentConnector;
    Controller currentController;
    int[] commandReadersIndices;

    void Awake()
    {
        Instance = this;
        commandReadersIndices = new int[15];
    }

    void Start()
    {
        mainUISystem.OnSwitchToOperatorScene += MainUISystem_OnSwitchToOperatorScene;
        operatorUISystem.OnSwitchToMainScene += OperatorUISystem_OnSwitchToMainScene;
    }

    private void OperatorUISystem_OnSwitchToMainScene()
    {
        operatorData = null;
        commandReadersIndices = new int[15];
        currentBrush = null;
        currentConnector = null;
        currentController = null;
    }

    private void MainUISystem_OnSwitchToOperatorScene(LevelDataSO levelData_, OperatorDataSO operatorData_)
    {
        operatorData = operatorData_;
        List<ConnectorData> connectorDatas = operatorData_.connectorDatas;
        List<ControllerData> controllerDatas = operatorData_.controllerDatas;

        for (int i = 0; i < connectorDatas.Count; i++)
        {
            int index = connectorDatas[i].cellIndex;
            HexCell cell = operatorSystem.GetCellFromIndex(index);
            Connector connector = Instantiate(pfConnector);
            connector.Setup(cell);
            connector.connectorData = connectorDatas[i];
            processSystem.OnCreateNewConnectorByData(connector);
        }

        for (int i = 0; i < controllerDatas.Count; i++)
        {
            int cellIndex = controllerDatas[i].cellIndex;
            int consoleIndex = controllerDatas[i].consoleIndex;

            HexCell cell = operatorSystem.GetCellFromIndex(cellIndex);
            Controller controller = Instantiate(pfController);
            controller.Setup(cell, controllerDatas[i]);
            // index
            operatorUISystem.InitConsole(controller, controllerDatas[i].consoleData);
            processSystem.OnCreateNewControllerByData(controller);
            commandReadersIndices[consoleIndex - 1] = consoleIndex; 
        }
    }

    public void InitBrushFromData(BrushData brushData_, BrushBtn brushBtn_)
    {
        int index = brushData_.cellIndex;
        HexCell cell = operatorSystem.GetCellFromIndex(index);

        switch (brushData_.type)
        {
            case BrushType.Coloring:
                ColorBrush colorBrush = Instantiate(pfColorBrush);
                colorBrush.SetupFromData(cell, brushData_, brushBtn_);
                processSystem.OnCreateNewBrush(colorBrush);
                break;
            case BrushType.Line:
                LineBrush lineBrush = Instantiate(pfLineBrush);
                lineBrush.SetupFromData(cell, brushData_, brushBtn_);
                processSystem.OnCreateNewBrush(lineBrush);
                break;
        }
    }

    void Update()
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }

        HandleMouseDrag();
        HandleControllerBuilding();
        HandleBrushBuilding();
        HandleConnectorBuilding();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && !InputHelper.IsMouseOverUIObject())
        {
            IMouseAction mouseDrag = InputHelper.GetIMouseDragUnderPosition2D();
            if (mouseDrag != null)
            {
                mouseDrag.MouseAction_Drag();
                return;
            }
        }
    }

    #region Color Picker
    /*    void HandleColorPickerBuilding()
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
    /*    public void CreateColorPicker(ColorSO color_)
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }
        currentColorPicker = Instantiate(pfColorPicker);
        currentColorPicker.Setup(color_);
    }*/
    #endregion
    void HandleBrushBuilding()
    {
        if (currentBrush != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    currentBrush.OnDestroyByPlayer();
                    OnDestoryBrush?.Invoke(currentBrush);
                    Destroy(currentBrush.gameObject);
                }
                else
                {
                    HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                    if (cell != null)
                    {
                        if (cell.IsEmpty())
                        {
                            currentBrush.Setup(cell);
                            currentBrush = null;
                            return;
                        }
                        else
                        {
                            currentBrush.OnDestroyByPlayer();
                            OnDestoryBrush?.Invoke(currentBrush);
                            Destroy(currentBrush.gameObject);
                        }
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
                else
                {
                    HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                    if (cell != null)
                    {
                        if (cell.IsEmpty())
                        {
                            currentConnector.Setup(cell);
                            currentConnector = null;
                            return;
                        }
                        else
                        {
                            OnDestoryConnector?.Invoke(currentConnector);
                            Destroy(currentConnector.gameObject);
                        }
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
                currentController.Rotate(RotateDirection.CounterClockwise);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                currentController.Rotate(RotateDirection.Clockwise);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    commandReadersIndices[currentController.index - 1] = 0;
                    OnDestoryController?.Invoke(currentController);
                    Destroy(currentController.gameObject);
                }else
                {
                    HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                    if (cell != null)
                    {
                        if (cell.IsEmpty())
                        {
                            currentController.Setup(cell);
                            currentController = null;
                            return;
                        }
                        else
                        {
                            commandReadersIndices[currentController.index - 1] = 0;
                            OnDestoryController?.Invoke(currentController);
                            Destroy(currentController.gameObject);
                        }
                    }
                }
            }

            currentController.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }
    }

    public void CreateNewController()
    {
        currentController = Instantiate(pfController);
        currentController.SetIndex(InputHelper.GetIndexFromIndicesArray(commandReadersIndices));
        OnCreateNewController?.Invoke(currentController);
    }

    public void CreateNewBrush(BrushBtn brushBtn_)
    {
        switch (brushBtn_.brushType)
        {
            case BrushType.Coloring:
                currentBrush = Instantiate(pfColorBrush);
                currentBrush.SetupFromBtn(brushBtn_);
                break;
            case BrushType.Line:
                currentBrush = Instantiate(pfLineBrush);
                currentBrush.SetupFromBtn(brushBtn_);
                break;
        }

        OnCreateNewBrush?.Invoke(currentBrush);
    }

    public void CreateNewConnecter()
    {
        currentConnector = Instantiate(pfConnector);
        OnCreateNewConnector?.Invoke(currentConnector);
    }

    public void SetCurrentTrackingBrush(Brush brush) => currentBrush = brush;
    public void SetCurrentTrackingConnector(Connector connector) => currentConnector = connector;
    public void SetCurrentTrackingController(Controller controller) => currentController = controller;
}

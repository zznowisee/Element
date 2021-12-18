using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildSystem : MonoBehaviour
{
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
    ColorPicker currentColorPicker;
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
        commandReadersIndices = new int[15];
        currentBrush = null;
        currentColorPicker = null;
        currentConnector = null;
        currentController = null;
    }

    private void MainUISystem_OnSwitchToOperatorScene(LevelDataSO levelData_, OperatorDataSO operatorData_)
    {
        List<BrushData> brushDatas = operatorData_.brushDatas;
        List<ConnectorData> connectorDatas = operatorData_.connectorDatas;
        List<ControllerData> controllerDatas = operatorData_.controllerDatas;
        for (int i = 0; i < brushDatas.Count; i++)
        {
            int index = brushDatas[i].cellIndex;
            ColorSO colorSO = brushDatas[i].colorSO;
            HexCell cell = operatorSystem.GetCellFromIndex(index);
            switch (brushDatas[i].type)
            {
                case BrushType.Coloring:
                    ColorBrush colorBrush = Instantiate(pfColorBrush);
                    colorBrush.Setup(cell, colorSO, BrushType.Coloring);
                    colorBrush.brushData = brushDatas[i];
                    processSystem.OnCreateNewBrush(colorBrush);
                    break;
                case BrushType.Line:
                    LineBrush lineBrush = Instantiate(pfLineBrush);
                    lineBrush.Setup(cell, colorSO, BrushType.Line);
                    lineBrush.brushData = brushDatas[i];
                    processSystem.OnCreateNewBrush(lineBrush);
                    break;
            }
        }

        for (int i = 0; i < connectorDatas.Count; i++)
        {
            int index = connectorDatas[i].cellIndex;
            HexCell cell = operatorSystem.GetCellFromIndex(index);
            Connector connector = Instantiate(pfConnector);
            connector.Setup(cell);
            connector.connectorData = connectorDatas[i];
            processSystem.OnCreateNewConnector(connector);
        }

        for (int i = 0; i < controllerDatas.Count; i++)
        {
            int cellIndex = controllerDatas[i].cellIndex;
            int consoleIndex = controllerDatas[i].consoleIndex;

            HexCell cell = operatorSystem.GetCellFromIndex(cellIndex);
            Controller controller = Instantiate(pfController);
            controller.direction = controllerDatas[i].direction;
            controller.SetIndex(consoleIndex);
            controller.Setup(cell);
            controller.controllerData = controllerDatas[i];
            // index
            operatorUISystem.InitConsole(operatorData_.consoleDatas[i].commandSOs, controller, operatorData_.consoleDatas[i]);
            processSystem.OnCreateNewController(controller);

            commandReadersIndices[consoleIndex - 1] = consoleIndex; 
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
        HandleColorPickerBuilding();
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

    void HandleBrushBuilding()
    {
        if (currentBrush != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (InputHelper.IsMouseOverUIObject())
                {
                    currentBrush.brushBtn.AddBackBrush();

                    OnDestoryBrush?.Invoke(currentBrush);
                    Destroy(currentBrush.gameObject);
                }

                HexCell cell = InputHelper.GetHexCellUnderPosition3D();
                if (cell != null)
                {
                    if (cell.CanInitNewBrush())
                    {
                        currentBrush.Setup(cell, currentBrush.colorSO, BrushType.Coloring);
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
                        currentConnector.Setup(cell);
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
                    commandReadersIndices[currentController.index - 1] = 0;
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

    public void CreateNewController()
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }
        currentController = Instantiate(pfController);
        int index = InputHelper.GetIndexFromIndicesArray(commandReadersIndices);
        currentController.SetIndex(index);
        OnCreateNewController?.Invoke(currentController);
    }

    public void CreateNewBrush(BrushBtn brushBtn_)
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }
        switch (brushBtn_.brushType)
        {
            case BrushType.Coloring:
                currentBrush = Instantiate(pfColorBrush);
                currentBrush.SpawnFromBtn(brushBtn_.colorSO, brushBtn_.brushType, brushBtn_);
                break;
            case BrushType.Line:
                currentBrush = Instantiate(pfLineBrush);
                currentBrush.SpawnFromBtn(brushBtn_.colorSO, brushBtn_.brushType, brushBtn_);
                break;
        }

        OnCreateNewBrush?.Invoke(currentBrush);
    }

    public void CreateNewConnecter()
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }
        currentConnector = Instantiate(pfConnector);
        OnCreateNewConnector?.Invoke(currentConnector);
    }

    public void CreateColorPicker(ColorSO color_)
    {
        if (!ProcessSystem.Instance.CanOperate())
        {
            return;
        }
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

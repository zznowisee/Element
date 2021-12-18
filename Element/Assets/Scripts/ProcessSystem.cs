using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessSystem : MonoBehaviour
{

    public OperatorDataSO current;

    public enum ProcessType
    {
        EDIT,
        PLAY,
        PAUSE,
        STEP
    }

    public enum ProcessState
    {
        RUNNING,
        WAITING,
        NOTSTART,
        WARNING
    }

    [SerializeField] ProcessType processType;
    [SerializeField] ProcessState processState;

    public event Action<int> OnReadNextCommandLine;
    public event Action OnFinishAllCommandsOrWarning;
    public event Action OnSwitchToMainScene;
    public event Action OnPlayerWin;
    public static ProcessSystem Instance { get; private set; }

    [Header("Command SO")]
    [SerializeField] CommandSO splitSO;
    [SerializeField] CommandSO brushCWRotateSO;
    [SerializeField] CommandSO brushCCWRotateSO;
    [SerializeField] CommandSO connectSO;
    [SerializeField] CommandSO goAheadSO;
    [SerializeField] CommandSO backSO;
    [SerializeField] CommandSO dropSO;
    [SerializeField] CommandSO pickSO;
    [SerializeField] CommandSO delaySO;
    [SerializeField] CommandSO controllerCCWSO;
    [SerializeField] CommandSO controllerCWSO;

    Dictionary<CommandSO, Action<ICommandReader>> commandDictionary;

    public List<Controller> controllers;
    public List<Connector> connectors;
    public List<Brush> brushes;
    public List<HexCell> recordCells;

    public float commandSpacingTime = .3f;
    public float commandDurationTime = .3f;

    int targetNum = 0;
    int currentNum;
    [HideInInspector] public int commandLineIndex = 0;
    [HideInInspector] public int commandLineMaxIndex = 0;

    [SerializeField] OperatorUISystem operatorUISystem;
    [SerializeField] MainUISystem mainUISystem;
    [SerializeField] HexMap hexMap;

    [SerializeField] ProductLine pfProductLine;
    [SerializeField] ProductCell pfProductCell;

    List<CheckProductColorCell> productColorCells;
    List<CheckProductHalfLine> productHalfLines;
    List<CheckProductColorCell> finishedColorCells;
    List<CheckProductHalfLine> finishedHalfLines;
    void Awake()
    {
        Instance = this;
        commandDictionary = new Dictionary<CommandSO, Action<ICommandReader>>();
        connectors = new List<Connector>();
        recordCells = new List<HexCell>();
        brushes = new List<Brush>();
        controllers = new List<Controller>();

        productColorCells = new List<CheckProductColorCell>();
        productHalfLines = new List<CheckProductHalfLine>();
        finishedColorCells = new List<CheckProductColorCell>();
        finishedHalfLines = new List<CheckProductHalfLine>();

        commandDictionary[splitSO] = Split;
        commandDictionary[brushCWRotateSO] = ClockwiseRotate;
        commandDictionary[brushCCWRotateSO] = CounterClockwiseRotate;
        commandDictionary[connectSO] = Connect;
        commandDictionary[dropSO] = Drop;
        commandDictionary[pickSO] = Pick;
        commandDictionary[delaySO] = Delay;
        commandDictionary[goAheadSO] = GoAhead;
        commandDictionary[backSO] = Back;
        commandDictionary[controllerCWSO] = ControllerCWRotate;
        commandDictionary[controllerCCWSO] = ControllerCCWRotate;

        processType = ProcessType.EDIT;
        processState = ProcessState.NOTSTART;
    }

    public bool CanOperate()
    {
        return processType == ProcessType.EDIT;
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewBrush += OnCreateNewBrush;
        BuildSystem.Instance.OnDestoryBrush += OnDestroyBrush;
        BuildSystem.Instance.OnCreateNewConnector += OnCreateNewConnector;
        BuildSystem.Instance.OnDestoryConnector += BuildSystem_OnDestoryConnector;
        BuildSystem.Instance.OnCreateNewController += OnCreateNewController;
        BuildSystem.Instance.OnDestoryController += OnDestroyController;

        operatorUISystem.OnSwitchToMainScene += OperatorUISystem_OnSwitchToMainScene;
        mainUISystem.OnSwitchToOperatorScene += MainUISystem_OnSwitchToOperatorScene;
    }

    private void MainUISystem_OnSwitchToOperatorScene(LevelDataSO levelData_, OperatorDataSO operatorData_)
    {
        current = operatorData_;

        for (int i = 0; i < levelData_.productData.cells.Length; i++)
        {
            CellData data = levelData_.productData.cells[i];
            Vector2Int coord = data.coord + hexMap.centerCellCoord;
            HexCell cell = hexMap.coordCellDictionary[coord];
            ProductCell productCell = Instantiate(pfProductCell, hexMap.productHolder);
            productCell.Setup(cell.transform.position, data.buildColor.color);

            productColorCells.Add(new CheckProductColorCell(cell, data.drawColor));
        }
        for (int i = 0; i < levelData_.productData.lines.Length; i++)
        {
            LineData data = levelData_.productData.lines[i];
            Vector2Int aCoord = data.pointA + hexMap.centerCellCoord;
            Vector2Int bCoord = data.pointB + hexMap.centerCellCoord;
            HexCell start = hexMap.coordCellDictionary[aCoord];
            HexCell end = hexMap.coordCellDictionary[bCoord];

            ProductLine productLine = Instantiate(pfProductLine);
            productLine.Setup(start.transform.position, end.transform.position, data.buildColor.color);
            productLine.transform.SetParent(hexMap.productHolder, true);

            productHalfLines.Add(new CheckProductHalfLine(start, end, data.drawColor));
            productHalfLines.Add(new CheckProductHalfLine(end, start, data.drawColor));
        }
    }

    private void OperatorUISystem_OnSwitchToMainScene()
    {
        current.brushDatas.Clear();
        current.connectorDatas.Clear();
        current.controllerDatas.Clear();

        for (int i = 0; i < controllers.Count; i++)
        {
            current.controllerDatas.Add(controllers[i].controllerData);
            controllers[i].OnSwitchToMainScene();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            current.brushDatas.Add(brushes[i].brushData);
            brushes[i].OnSwitchToMainScene();
        }
        for (int i = 0; i < connectors.Count; i++)
        {
            current.connectorDatas.Add(connectors[i].connectorData);
            connectors[i].OnSwitchToMainScene();
        }
        controllers.Clear();
        brushes.Clear();
        connectors.Clear();
        current = null;
    }

    private void OnDestroyController(Controller controller)
    {
        controllers.Remove(controller);
        controller.OnFinishCommand -= OnReaderFinishCommand;
        controller.OnWarning -= OnWarning;
    }

    public void OnCreateNewController(Controller controller)
    {
        controllers.Add(controller);
        controller.OnFinishCommand += OnReaderFinishCommand;
        controller.OnWarning += OnWarning;
    }

    private void OnWarning(Vector3 position, WarningType warningType)
    {
        StopAllCoroutines();
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].StopAllCoroutines();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].StopAllCoroutines();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].StopAllCoroutines();
        }

        OnFinishAllCommandsOrWarning?.Invoke();
        processState = ProcessState.WARNING;
        processType = ProcessType.PAUSE;
        TooltipSystem.Instance.ShowWarning(position, warningType);
    }

    public void OnDestroyBrush(Brush brush)
    {
        brushes.Remove(brush);
        brush.OnWarning -= OnWarning;
        switch (brush.brushType)
        {
            case BrushType.Coloring:
                brush.GetComponent<ColorBrush>().OnColoringCell -= OnColoringCellColoring;
                break;
            case BrushType.Line:
                brush.GetComponent<LineBrush>().OnDrawingLine -= OnLineBrushDrawingLine;
                break;
        }
    }

    public void OnCreateNewBrush(Brush brush)
    {
        brushes.Add(brush);
        brush.OnWarning += OnWarning;
        switch (brush.brushType)
        {
            case BrushType.Coloring:
                brush.GetComponent<ColorBrush>().OnColoringCell += OnColoringCellColoring;
                break;
            case BrushType.Line:
                brush.GetComponent<LineBrush>().OnDrawingLine += OnLineBrushDrawingLine;
                break;
        }
    }

    private void OnLineBrushDrawingLine(HexCell from_, HexCell to_, ColorSO colorSO_)
    {
        for (int i = 0; i < productHalfLines.Count; i++)
        {
            if(productHalfLines[i].from == from_ && productHalfLines[i].to == to_ && productHalfLines[i].colorSO == colorSO_)
            {
                CheckProductHalfLine ft = new CheckProductHalfLine(from_, to_, colorSO_);
                CheckProductHalfLine tf = new CheckProductHalfLine(to_, from_, colorSO_);
                productHalfLines.Remove(ft);
                productHalfLines.Remove(tf);
                finishedHalfLines.Add(ft);
                finishedHalfLines.Add(ft);
                break;
            }
        }
    }

    private void OnColoringCellColoring(HexCell coloringCell_, ColorSO colorSO_)
    {
        for (int i = 0; i < productColorCells.Count; i++)
        {
            if(productColorCells[i].cell == coloringCell_ && productColorCells[i].colorSO == colorSO_)
            {
                CheckProductColorCell pc = new CheckProductColorCell(coloringCell_, colorSO_);
                finishedColorCells.Add(pc);
                productColorCells.Remove(pc);
                for (int j = 0; j < finishedHalfLines.Count; j++)
                {
                    if(finishedHalfLines[j].from == coloringCell_)
                    {
                        productHalfLines.Add(finishedHalfLines[j]);
                    }
                }
                break;
            }
        }
    }

    public void BuildSystem_OnDestoryConnector(Connector connector)
    {
        connectors.Remove(connector);
        connector.OnWarning -= OnWarning;
    }

    public void OnCreateNewConnector(Connector connector)
    {
        connectors.Add(connector);
        connector.OnWarning += OnWarning;
    }

    void ControllerCWRotate(ICommandReader reader) => reader.ControllerCWRotate();
    void ControllerCCWRotate(ICommandReader reader) => reader.ControllerCCWRotate();
    void Split(ICommandReader reader) => reader.Split();
    void Connect(ICommandReader reader) => reader.Connect();
    void ClockwiseRotate(ICommandReader reader) => reader.ConnectorCWRotate();
    void CounterClockwiseRotate(ICommandReader reader) => reader.ConnectorCCWRotate();
    void Drop(ICommandReader reader) => reader.Putdown();
    void Pick(ICommandReader reader) => reader.Putup();
    void Delay(ICommandReader reader) => reader.Delay();
    void GoAhead(ICommandReader reader) => reader.Push();
    void Back(ICommandReader reader) => reader.Pull();

    public void OnReaderFinishCommand()
    {
        currentNum++;
        //
        if (CheckFinish())
        {
            print("WIN");
            return;
        }
        //





        if (processState != ProcessState.WARNING)
        {
            if (targetNum == currentNum)
            {
                switch (processType)
                {
                    case ProcessType.PLAY:
                        StartCoroutine(Spacing(commandSpacingTime));
                        break;
                    case ProcessType.STEP:
                        print("PAUSE");
                        processType = ProcessType.PAUSE;
                        processState = ProcessState.WAITING;
                        break;
                    case ProcessType.PAUSE:
                        print("PAUSE");
                        processState = ProcessState.WAITING;
                        break;
                }
            }
        }
    }

    IEnumerator Spacing(float spacingTime)
    {
        yield return new WaitForSeconds(spacingTime);
        RunOnce();
    }

    void RunOnce()
    {
        if (commandLineIndex <= commandLineMaxIndex)
        {
            OnReadNextCommandLine?.Invoke(commandLineIndex);
            targetNum = controllers.Count;
            currentNum = 0;

            for (int i = 0; i < controllers.Count; i++)
            {
                ICommandReader readerObj = controllers[i].GetComponent<ICommandReader>();
                CommandSO cmd = operatorUISystem.GetEachReaderCommandSO(commandLineIndex, readerObj);
                if (cmd != null)
                {
                    commandDictionary[cmd].Invoke(readerObj);
                }
                else
                {
                    readerObj.Delay();
                }
            }

            commandLineIndex++;
            if(commandLineIndex > commandLineMaxIndex)
            {
                processType = ProcessType.PAUSE;
                OnFinishAllCommandsOrWarning?.Invoke();
            }
        }
    }

    void Record()
    {
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].Record();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].Record();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].Record();
        }
    }

    void Read()
    {
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].ClearCurrentInfo();
        }
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].ReadPreviousInfo();
        }

        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].ClearCurrentInfo();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].ReadPreviousInfo();
        }

        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].ClearCurrentInfo();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].ReadPreviousInfo();
        }

        for (int i = 0; i < recordCells.Count; i++)
        {
            recordCells[i].ResetCell();
        }
        recordCells.Clear();
    }

    public void PlayPause(bool isPlayCommand)
    {
        //play btn
        if (isPlayCommand)
        {
            commandLineMaxIndex = operatorUISystem.GetCommandLineMaxIndex();
            switch (processType)
            {
                case ProcessType.EDIT:
                    Record();
                    RunOnce();
                    processType = ProcessType.PLAY;
                    processState = ProcessState.RUNNING;
                    break;
                case ProcessType.PAUSE:
                    processType = ProcessType.PLAY;
                    processState = ProcessState.RUNNING;
                    RunOnce();
                    break;
                case ProcessType.STEP:
                    processType = ProcessType.STEP;
                    break;
            }
        }
        //pause btn
        else
        {
            processType = ProcessType.PAUSE;
        }
    }

    public void Step()
    {
        switch (processType)
        {
            case ProcessType.EDIT:
                processType = ProcessType.STEP;
                Record();
                commandLineMaxIndex = operatorUISystem.GetCommandLineMaxIndex();
                processState = ProcessState.RUNNING;
                RunOnce();
                break;
            case ProcessType.PAUSE:
                processType = ProcessType.STEP;
                processState = ProcessState.RUNNING;
                RunOnce();
                break;
            case ProcessType.PLAY:
                processType = ProcessType.STEP;
                break;
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].StopAllCoroutines();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].StopAllCoroutines();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].StopAllCoroutines();
        }
        commandLineIndex = 0;
        currentNum = targetNum = 0;
        // read all infos
        Read();
        for (int i = 0; i < finishedColorCells.Count; i++)
        {
            productColorCells.Add(finishedColorCells[i]);
        }
        finishedColorCells.Clear();
        for (int i = 0; i < finishedHalfLines.Count; i++)
        {
            productHalfLines.Add(finishedHalfLines[i]);
        }
        finishedHalfLines.Clear();
        processType = ProcessType.EDIT;
        processState = ProcessState.NOTSTART;
        TooltipSystem.Instance.HideWarning();
    }

    public bool CheckFinish()
    {
        if(productHalfLines.Count == 0 && productColorCells.Count == 0)
        {
            OnPlayerWin?.Invoke();
            return true;
        }

        return false;
    }

    public struct CheckProductHalfLine
    {
        public HexCell from;
        public HexCell to;
        public ColorSO colorSO;
        public CheckProductHalfLine(HexCell from_, HexCell to_, ColorSO colorSO_)
        {
            from = from_;
            to = to_;
            colorSO = colorSO_;
        }
        public static bool operator ==(CheckProductHalfLine a, CheckProductHalfLine b)
        {
            return (a.from == b.from && a.to == b.to) || (a.to == b.from && a.from == b.to);
        }

        public static bool operator !=(CheckProductHalfLine a, CheckProductHalfLine b)
        {
            return !(a == b);
        }
    }

    public struct CheckProductColorCell
    {
        public HexCell cell;
        public ColorSO colorSO;
        public CheckProductColorCell(HexCell cell_, ColorSO colorSO_)
        {
            cell = cell_;
            colorSO = colorSO_;
        }
        public static bool operator ==(CheckProductColorCell a, CheckProductColorCell b)
        {
            return a.cell == b.cell && a.colorSO == b.colorSO;
        }

        public static bool operator !=(CheckProductColorCell a, CheckProductColorCell b)
        {
            return !(a == b);
        }
    }
}

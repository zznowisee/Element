using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessSystem : MonoBehaviour
{

    public SolutionData solutionData;
    public LevelData levelData;

    public enum ProcessType
    {
        EDIT,
        PLAY,
        PAUSE,
        STEP
    }

    public enum ProcessState
    {
        Running,
        Waiting,
        NotStart,
        Warning
    }

    [SerializeField] ProcessType processType;
    [SerializeField] ProcessState processState;

    public event Action<int> OnReadNextCommandLine;
    public event Action OnFinishAllCommandsOrWarning;
    public event Action<LevelData> OnLevelComplete;
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

    int totalWaitingNum;
    [HideInInspector] public int commandLineIndex = 0;

    [SerializeField] OperatorUISystem operatorUISystem;
    [SerializeField] MainUISystem mainUISystem;
    [SerializeField] HexMap hexMap;

    [SerializeField] ProductLine pfProductLine;
    [SerializeField] ProductCell pfProductCell;

    [SerializeField] List<CheckProductColorCell> totalColorCells;
    [SerializeField] List<CheckProductHalfLine> totalHalfLines;

    [SerializeField] List<CheckProductColorCell> productColorCells;
    [SerializeField] List<CheckProductHalfLine> productHalfLines;

    [SerializeField] List<CheckProductColorCell> finishedColorCells;
    [SerializeField] List<CheckProductHalfLine> finishedHalfLines;

    public bool solutionCompleted = false;
    void Awake()
    {
        Instance = this;
        commandDictionary = new Dictionary<CommandSO, Action<ICommandReader>>();
        connectors = new List<Connector>();
        recordCells = new List<HexCell>();
        brushes = new List<Brush>();
        controllers = new List<Controller>();

        totalColorCells = new List<CheckProductColorCell>();
        totalHalfLines = new List<CheckProductHalfLine>();
        productColorCells = new List<CheckProductColorCell>();
        productHalfLines = new List<CheckProductHalfLine>();
        finishedColorCells = new List<CheckProductColorCell>();
        finishedHalfLines = new List<CheckProductHalfLine>();

        commandDictionary[splitSO] = Split;
        commandDictionary[brushCWRotateSO] = ConnectorCR;
        commandDictionary[brushCCWRotateSO] = ConnectorCCR;
        commandDictionary[connectSO] = Connect;
        commandDictionary[dropSO] = PutDown;
        commandDictionary[pickSO] = PutUp;
        commandDictionary[delaySO] = Delay;
        commandDictionary[goAheadSO] = Push;
        commandDictionary[backSO] = Pull;
        commandDictionary[controllerCWSO] = SelfCR;
        commandDictionary[controllerCCWSO] = SelfCCR;

        processType = ProcessType.EDIT;
        processState = ProcessState.NotStart;
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
        BuildSystem.Instance.OnDestoryConnector += OnDestoryConnector;
        BuildSystem.Instance.OnCreateNewController += OnCreateNewController;
        BuildSystem.Instance.OnDestoryController += OnDestroyController;

        operatorUISystem.OnSwitchToMainScene += OperatorUISystem_OnSwitchToMainScene;
        mainUISystem.OnLoadSolution += MainUISystem_OnSwitchToOperatorScene;
    }

    private void MainUISystem_OnSwitchToOperatorScene(LevelBuildDataSO levelBuildDataSO_, LevelData levelData_, SolutionData solutionData_)
    {
        solutionData = solutionData_;
        levelData = levelData_;
        for (int i = 0; i < levelBuildDataSO_.productData.cells.Length; i++)
        {
            CellData data = levelBuildDataSO_.productData.cells[i];
            Vector2Int coord = data.coord + hexMap.centerCellCoord;
            HexCell cell = hexMap.coordCellDictionary[coord];
            ProductCell productCell = Instantiate(pfProductCell, hexMap.productHolder);
            productCell.Setup(cell.transform.position, data.color.initColor);

            productColorCells.Add(new CheckProductColorCell(cell, data.color));
            totalColorCells.Add(new CheckProductColorCell(cell, data.color));
        }
        for (int i = 0; i < levelBuildDataSO_.productData.lines.Length; i++)
        {
            LineData data = levelBuildDataSO_.productData.lines[i];
            Vector2Int aCoord = data.pointA + hexMap.centerCellCoord;
            Vector2Int bCoord = data.pointB + hexMap.centerCellCoord;
            HexCell start = hexMap.coordCellDictionary[aCoord];
            HexCell end = hexMap.coordCellDictionary[bCoord];

            ProductLine productLine = Instantiate(pfProductLine);
            productLine.Setup(start.transform.position, end.transform.position, data.color.initColor);
            productLine.transform.SetParent(hexMap.productHolder, true);

            productHalfLines.Add(new CheckProductHalfLine(start, end, data.color));
            productHalfLines.Add(new CheckProductHalfLine(end, start, data.color));
            totalHalfLines.Add(new CheckProductHalfLine(start, end, data.color));
            totalHalfLines.Add(new CheckProductHalfLine(end, start, data.color));
        }
    }

    private void OperatorUISystem_OnSwitchToMainScene()
    {
        solutionData.connectorDatas.Clear();
        solutionData.controllerDatas.Clear();

        for (int i = 0; i < controllers.Count; i++)
        {
            solutionData.controllerDatas.Add(controllers[i].controllerData);
            controllers[i].OnSwitchToMainScene();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].OnSwitchToMainScene();
        }
        for (int i = 0; i < connectors.Count; i++)
        {
            solutionData.connectorDatas.Add(connectors[i].connectorData);
            connectors[i].OnSwitchToMainScene();
        }

        totalColorCells.Clear();
        totalHalfLines.Clear();
        productColorCells.Clear();
        productHalfLines.Clear();
        finishedColorCells.Clear();
        finishedHalfLines.Clear();
        controllers.Clear();
        brushes.Clear();
        connectors.Clear();
        solutionCompleted = false;
        solutionData = null;
        levelData = null;
    }

    private void OnDestroyController(Controller controller)
    {
        controllers.Remove(controller);
        solutionData.controllerDatas.Remove(controller.controllerData);
        controller.OnFinishCommand -= OnReaderFinishCommand;
        controller.OnWarning -= OnWarning;
    }

    public void OnCreateNewController(Controller controller)
    {
        controllers.Add(controller);
        solutionData.controllerDatas.Add(controller.controllerData);
        controller.OnFinishCommand += OnReaderFinishCommand;
        controller.OnWarning += OnWarning;
    }

    private void OnWarning(Vector3 position, WarningType warningType)
    {
        processState = ProcessState.Warning;
        processType = ProcessType.PAUSE;
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
        TooltipSystem.Instance.ShowWarning(position, warningType);
    }

    public void OnDestroyBrush(Brush brush)
    {
        brushes.Remove(brush);
        brush.OnWarning -= OnWarning;
        switch (brush.brushData.type)
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
        switch (brush.brushData.type)
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
        bool partOfProduct = false;
        for (int i = 0; i < finishedHalfLines.Count; i++)
        {
            if(finishedHalfLines[i].from == from_ && finishedHalfLines[i].to == to_)
            {
                if(finishedHalfLines[i].colorSO != colorSO_)
                {
                    CheckProductHalfLine ft = new CheckProductHalfLine(from_, to_, colorSO_);
                    CheckProductHalfLine tf = new CheckProductHalfLine(to_, from_, colorSO_);
                    finishedHalfLines.Remove(ft);
                    finishedHalfLines.Remove(tf);
                    productHalfLines.Add(ft);
                    productHalfLines.Add(tf);
                    break;
                }
            }
        }

        for (int i = 0; i < totalHalfLines.Count; i++)
        {
            if(totalHalfLines[i].from == from_ && totalHalfLines[i].to == to_)
            {
                partOfProduct = true;
                if (totalHalfLines[i].colorSO == colorSO_)
                {
                    CheckProductHalfLine ft = new CheckProductHalfLine(from_, to_, colorSO_);
                    CheckProductHalfLine tf = new CheckProductHalfLine(to_, from_, colorSO_);
                    productHalfLines.Remove(ft);
                    productHalfLines.Remove(tf);
                    if(!finishedHalfLines.Contains(ft) && !finishedHalfLines.Contains(tf))
                    {
                        finishedHalfLines.Add(ft);
                        finishedHalfLines.Add(tf);
                    }
                    break;
                }
            }
        }

        if (!partOfProduct)
        {
            OnWarning(to_.transform.position, WarningType.WrongLine);
        }
    }

    private void OnColoringCellColoring(HexCell coloringCell_, ColorSO colorSO_)
    {
        bool partOfProduct = false;
        for (int i = 0; i < finishedColorCells.Count; i++)
        {
            if(finishedColorCells[i].cell == coloringCell_)
            {
                if(finishedColorCells[i].colorSO != colorSO_)
                {
                    CheckProductColorCell cc = new CheckProductColorCell() { cell = finishedColorCells[i].cell, colorSO = finishedColorCells[i].colorSO };
                    productColorCells.Add(cc);
                    finishedColorCells.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = 0; i < totalColorCells.Count; i++)
        {
            if(totalColorCells[i].cell == coloringCell_)
            {
                partOfProduct = true;
                if (totalColorCells[i].colorSO == colorSO_)
                {
                    CheckProductColorCell pc = new CheckProductColorCell(coloringCell_, colorSO_);
                    finishedColorCells.Add(pc);
                    productColorCells.Remove(pc);
                    for (int j = 0; j < finishedHalfLines.Count; j++)
                    {
                        if (finishedHalfLines[j].from == coloringCell_)
                        {
                            productHalfLines.Add(finishedHalfLines[j]);
                        }
                    }
                    break;
                }
            }
        }

        if (!partOfProduct)
        {
            OnWarning(coloringCell_.transform.position, WarningType.WrongColoring);
        }
    }

    public void OnCreateNewConnectorByData(Connector connector)
    {
        connectors.Add(connector);
        connector.OnWarning += OnWarning;
    }

    public void OnCreateNewControllerByData(Controller controller)
    {
        controllers.Add(controller);
        controller.OnWarning += OnWarning;
        controller.OnFinishCommand += OnReaderFinishCommand;
    }

    public void OnDestoryConnector(Connector connector)
    {
        connectors.Remove(connector);
        solutionData.connectorDatas.Remove(connector.connectorData);
        connector.OnWarning -= OnWarning;
    }

    public void OnCreateNewConnector(Connector connector)
    {
        connectors.Add(connector);
        solutionData.connectorDatas.Add(connector.connectorData);
        connector.OnWarning += OnWarning;
    }

    void SelfCR(ICommandReader reader) => reader.RunCommand(CommandType.ControllerCR);
    void SelfCCR(ICommandReader reader) => reader.RunCommand(CommandType.ControllerCCR);
    void Split(ICommandReader reader) => reader.RunCommand(CommandType.Split);
    void Connect(ICommandReader reader) => reader.RunCommand(CommandType.Connect);
    void ConnectorCR(ICommandReader reader) => reader.RunCommand(CommandType.ConnectorCR);
    void ConnectorCCR(ICommandReader reader) => reader.RunCommand(CommandType.ConnectorCCR);
    void PutDown(ICommandReader reader) => reader.RunCommand(CommandType.PutDown);
    void PutUp(ICommandReader reader) => reader.RunCommand(CommandType.PutUp);
    void Delay(ICommandReader reader) => reader.RunCommand(CommandType.Delay);
    void Push(ICommandReader reader) => reader.RunCommand(CommandType.Push);
    void Pull(ICommandReader reader) => reader.RunCommand(CommandType.Pull);

    public void OnReaderFinishCommand()
    {
        totalWaitingNum++;

        if (CheckFinish())
        {
            if (commandLineIndex == operatorUISystem.GetCommandLineMaxIndex())
            {
                OnFinishAllCommandsOrWarning?.Invoke();
            }

            solutionCompleted = true;
            solutionData.complete = true;
            OnLevelComplete?.Invoke(levelData);
            processType = ProcessType.PAUSE;
            processState = ProcessState.Waiting;
        }

        if (processState != ProcessState.Warning)
        {
            if (totalWaitingNum == controllers.Count)
            {
                totalWaitingNum = 0;
                switch (processType)
                {
                    case ProcessType.PLAY:
                        StartCoroutine(Spacing(commandSpacingTime));
                        break;
                    case ProcessType.STEP:
                        processType = ProcessType.PAUSE;
                        processState = ProcessState.Waiting;
                        break;
                    case ProcessType.PAUSE:
                        processState = ProcessState.Waiting;
                        break;
                }
            }
        }
    }

    IEnumerator Spacing(float spacingTime)
    {
        processState = ProcessState.Waiting;
        yield return new WaitForSeconds(spacingTime);
        RunOnce();
    }

    void RunOnce()
    {
        int lineMaxIndex = operatorUISystem.GetCommandLineMaxIndex();
        if (commandLineIndex <= lineMaxIndex)
        {
            processState = ProcessState.Running;
            OnReadNextCommandLine?.Invoke(commandLineIndex);
            totalWaitingNum = 0;

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
                    readerObj.RunCommand(CommandType.Delay);
                }
            }

            commandLineIndex++;
            if(commandLineIndex > lineMaxIndex)
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
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].ClearCurrentInfo();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].ClearCurrentInfo();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].ReadPreviousInfo();
        }
        for (int i = 0; i < connectors.Count; i++)
        {
            connectors[i].ReadPreviousInfo();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].ReadPreviousInfo();
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
            switch (processType)
            {
                case ProcessType.EDIT:
                    Record();
                    RunOnce();
                    processType = ProcessType.PLAY;
                    break;
                case ProcessType.PAUSE:
                    RunOnce();
                    processType = ProcessType.PLAY;
                    break;
                case ProcessType.STEP:
                    processType = ProcessType.PLAY;
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
                Record();
                RunOnce();
                processType = ProcessType.STEP;
                break;
            case ProcessType.PAUSE:
                RunOnce();
                processType = ProcessType.STEP;
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
        totalWaitingNum = 0;
        // read all infos
        Read();
        finishedColorCells.ForEach(colorCell => productColorCells.Add(colorCell));
        finishedHalfLines.ForEach(halfLine => productHalfLines.Add(halfLine));
        finishedColorCells.Clear();
        finishedHalfLines.Clear();
        processType = ProcessType.EDIT;
        processState = ProcessState.NotStart;
        TooltipSystem.Instance.HideWarning();
    }

    public void OnFinishedExit()
    {
        commandLineIndex = totalWaitingNum = 0;
        productColorCells.Clear();
        finishedColorCells.Clear();
        productHalfLines.Clear();
        finishedHalfLines.Clear();
        totalColorCells.Clear();
        totalHalfLines.Clear();
        processType = ProcessType.EDIT;
        processState = ProcessState.NotStart;
        solutionCompleted = false;
        TooltipSystem.Instance.HideWarning();

        for (int i = 0; i < recordCells.Count; i++)
        {
            recordCells[i].ResetCell();
        }

        recordCells.Clear();
    }

    public bool CheckFinish()
    {
        return productHalfLines.Count == 0 && productColorCells.Count == 0 && !solutionCompleted && processState != ProcessState.Warning;
    }

    [Serializable]
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

    [Serializable]
    public struct CheckProductColorCell
    {
        public HexCell cell;
        public ColorSO colorSO;
        public CheckProductColorCell(HexCell cell_, ColorSO colorSO_)
        {
            cell = cell_;
            colorSO = colorSO_;
        }
        public static bool operator == (CheckProductColorCell a, CheckProductColorCell b)
        {
            return a.cell == b.cell && a.colorSO == b.colorSO;
        }

        public static bool operator !=(CheckProductColorCell a, CheckProductColorCell b)
        {
            return !(a == b);
        }
    }
}

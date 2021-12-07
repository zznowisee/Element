using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessSystem : MonoBehaviour
{
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
        NOTSTART
    }

    [SerializeField] ProcessType processType;
    [SerializeField] ProcessState processState;

    public event Action<int> OnReadNextCommandLine;
    public event Action OnFinishAllCommands;
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
    public List<HexCell> colorCells;
    public List<HexCell> lineCells;

    public float commandSpacingTime = .3f;
    public float commandDurationTime = .3f;

    int targetNum = 0;
    int currentNum;
    public int commandLineIndex = 0;
    public int commandLineMaxIndex = 0;

    UISystem uiSystem;
    void Awake()
    {
        Instance = this;
        commandDictionary = new Dictionary<CommandSO, Action<ICommandReader>>();
        connectors = new List<Connector>();
        colorCells = new List<HexCell>();
        lineCells = new List<HexCell>();
        brushes = new List<Brush>();
        controllers = new List<Controller>();

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

        uiSystem = FindObjectOfType<UISystem>();
    }

    public bool CanOperate()
    {
        return processType == ProcessType.EDIT;
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewBrush += BuildSystem_OnCreateNewBrush;
        BuildSystem.Instance.OnDestoryBrush += BuildSystem_OnDestoryBrush;
        BuildSystem.Instance.OnCreateNewConnector += BuildSystem_OnCreateNewConnector;
        BuildSystem.Instance.OnDestoryConnector += BuildSystem_OnDestoryConnector;
        BuildSystem.Instance.OnCreateNewController += BuildSystem_OnCreateNewController;
        BuildSystem.Instance.OnDestoryController += BuildSystem_OnDestoryController;
    }

    private void BuildSystem_OnDestoryController(Controller controller)
    {
        controllers.Remove(controller);
        controller.OnFinishCommand -= OnReaderFinishCommand;
    }

    private void BuildSystem_OnCreateNewController(Controller controller)
    {
        controllers.Add(controller);
        controller.OnFinishCommand += OnReaderFinishCommand;
    }

    private void BuildSystem_OnDestoryBrush(Brush brush)
    {
        brushes.Remove(brush);
    }

    private void BuildSystem_OnCreateNewBrush(Brush brush)
    {
        brushes.Add(brush);
    }

    private void BuildSystem_OnDestoryConnector(Connector connecter)
    {
        connectors.Remove(connecter);
    }

    private void BuildSystem_OnCreateNewConnector(Connector connecter)
    {
        connectors.Add(connecter);
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
                CommandSO cmd = uiSystem.GetEachReaderCommandSO(commandLineIndex, readerObj);
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
                OnFinishAllCommands?.Invoke();
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
            connectors[i].Read();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].Read();
        }
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].Read();
        }
    }

    public void PlayPause(bool isPlayCommand)
    {
        //play btn
        if (isPlayCommand)
        {
            commandLineMaxIndex = uiSystem.GetCommandLineMaxIndex();
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
                commandLineMaxIndex = uiSystem.GetCommandLineMaxIndex();
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

    void ClearColor()
    {
        for (int i = 0; i < colorCells.Count; i++)
        {
            HexCell cell = colorCells[i];
            cell.ClearColor();
        }

        colorCells.Clear();
    }

    void ClearLine()
    {
        for (int i = 0; i < lineCells.Count; i++)
        {
            HexCell cell = lineCells[i];
            cell.ClearLine();
        }

        lineCells.Clear();
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
        commandLineIndex = 0;
        // read all infos
        Read();
        ClearColor();
        ClearLine();

        processType = ProcessType.EDIT;
        processState = ProcessState.NOTSTART;
    }
}

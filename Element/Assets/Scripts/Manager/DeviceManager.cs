using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeviceManager : MonoBehaviour
{
    [Header("管理器")]
    [SerializeField] HexMap map;
    [Header("父级")]
    [SerializeField] Transform consolePanel;

    Dictionary<ICommandReleaser, CommandConsole> commandReaderConsoleDictionary;

    public static DeviceManager Instance { get; private set; }
    [Header("预制体")]
    [SerializeField] ColorBrush pfColorBrush;
    [SerializeField] LineBrush pfLineBrush;
    [SerializeField] Connector pfConnector;
    [SerializeField] Controller pfController;
    [SerializeField] CommandConsole pfCommandConsole;
    [SerializeField] Command pfCommand;
    [Header("指令SO")]
    [SerializeField] CommandSO putup;
    [SerializeField] CommandSO putdown;
    [SerializeField] CommandSO connect;
    [SerializeField] CommandSO split;
    [SerializeField] CommandSO connectorCCW;
    [SerializeField] CommandSO connectorCW;
    [SerializeField] CommandSO controllerCCW;
    [SerializeField] CommandSO controllerCW;
    [SerializeField] CommandSO delay;
    [SerializeField] CommandSO push;
    [SerializeField] CommandSO pull;

    public int TargetFinishNumber => commandReaderConsoleDictionary.Count;
    public int MaxLineIndex => GetCommandLineMaxIndex();


    public int[] commandReadersIndices;

    void Awake()
    {
        Instance = this;
        commandReadersIndices = new int[15];
        commandReaderConsoleDictionary = new Dictionary<ICommandReleaser, CommandConsole>();
    }

    void Start()
    {
        MainUI.Instance.OnSwitchToOperatingRoom += SwitchToOperatingRoom;
        ProcessManager.Instance.OnRunNewLine += RunNewLine;

        OperatingRoomUI.Instance.OnSwitchToMainScene += SwitchToMainScene;
        OperatingRoomUI.Instance.OnStop += ResetHighlightingCommands;
    }

    public Command NewCommand(Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(pfCommand, position, rotation, parent);
    }

    public Command NewCommand(Transform parent)
    {
        return Instantiate(pfCommand, parent);
    }

    void SwitchToMainScene()
    {
        print(commandReaderConsoleDictionary.Count);
        foreach(CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            Destroy(console.gameObject);
        }
        commandReaderConsoleDictionary.Clear();
        commandReadersIndices = new int[15];
    }

    void SwitchToOperatingRoom(LevelBuildDataSO levelBuildDataSO, LevelData levelData, SolutionData solutionData_)
    {
        print("Switch to operating room");
        List<ControllerData> controllerDatas = solutionData_.controllerDatas;
        List<ConnectorData> connectorDatas = solutionData_.connectorDatas;
        //controller and console
        print("controllerDatas.Count : " + controllerDatas.Count);
        for (int i = 0; i < controllerDatas.Count; i++)
        {
            ControllerData controllerData = controllerDatas[i];
            ConsoleData consoleData = controllerData.consoleData;

            HexCell cell = map.GetCellFromIndex(controllerData.cellIndex);
            int consoleIndex = controllerDatas[i].index;

            Controller controller = Instantiate(pfController);
            CommandConsole console = Instantiate(pfCommandConsole, consolePanel);
            controller.Rebuild(cell, controllerDatas[i]);
            Command[] rebuildCommands = new Command[consoleData.commandTypes.Length];

            for (int j = 0; j < rebuildCommands.Length; j++)
            {
                if (consoleData.commandTypes[j] == CommandType.Empty)
                    continue;

                CommandSO commandSO = GetCommandSOFromType(consoleData.commandTypes[j]);
                if (commandSO != null)
                {
                    rebuildCommands[j] = Instantiate(pfCommand, OperatingRoomUI.Instance.transform);
                    rebuildCommands[j].Setup(commandSO);
                }
            }

            console.Rebuild(controller, consoleData, rebuildCommands);
            commandReaderConsoleDictionary[controller] = console;
            // index
            commandReadersIndices[consoleIndex - 1] = consoleIndex;
            controller.OnDestoryByPlayer += ControllerDestoryByPlayer;

            ProcessManager.Instance.RegisterController(controller);
            DataManager.Instance.RegisterRebuildController(controller);
        }

        print(commandReaderConsoleDictionary.Count);
        //connector
        for (int i = 0; i < connectorDatas.Count; i++)
        {
            HexCell cell = map.GetCellFromIndex(connectorDatas[i].cellIndex);
            Connector connector = Instantiate(pfConnector);
            //bind data
            connector.Setup(cell);
            connector.data = connectorDatas[i];
            DataManager.Instance.RegisterRebuildConnector(connector);
        }
    }

    CommandSO GetCommandSOFromType(CommandType type)
    {
        switch (type)
        {
            case CommandType.Delay: return delay;
            case CommandType.Pull: return pull;
            case CommandType.Push: return push;
            case CommandType.Split: return split;
            case CommandType.Connect: return connect;
            case CommandType.PutUp: return putup;
            case CommandType.PutDown: return putdown;
            case CommandType.ConnectorCW: return connectorCW;
            case CommandType.ConnectorCCW: return connectorCCW;
            case CommandType.ControllerCW: return controllerCW;
            case CommandType.ControllerCCW: return controllerCCW;
            default: case CommandType.Empty: return null;
        }
    }

    public Controller NewController()
    {
        Controller controller = Instantiate(pfController);
        controller.transform.position = InputHelper.MouseWorldPositionIn2D;
        CommandConsole console = Instantiate(pfCommandConsole, consolePanel);
        controller.OnDestoryByPlayer += OnControllerDestory;

        controller.SetIndex(GetIndexFromIndicesArray());
        console.Setup(controller);

        commandReaderConsoleDictionary[controller] = console;
        controller.OnDestoryByPlayer += ControllerDestoryByPlayer;
        //OnCreateNewController?.Invoke(controller);
        ProcessManager.Instance.RegisterController(controller);
        DataManager.Instance.RegisterNewDevice(controller);
        return controller;
    }

    void ControllerDestoryByPlayer(Controller controller)
    {
        commandReaderConsoleDictionary.Remove(controller);
        commandReadersIndices[controller.index - 1] = 0;
    }

    public Brush NewBrush(BrushType brushType, ColorType colorType)
    {
        switch (brushType)
        {
            default:
            case BrushType.Coloring:
                var cb = Instantiate(pfColorBrush);
                cb.transform.position = InputHelper.MouseWorldPositionIn2D;
                cb.Init(brushType, colorType);
                DataManager.Instance.RegisterNewDevice(cb);
                return cb;
            case BrushType.Line:
                var lb = Instantiate(pfLineBrush);
                lb.transform.position = InputHelper.MouseWorldPositionIn2D;
                lb.Init(brushType, colorType);
                DataManager.Instance.RegisterNewDevice(lb);
                return lb;
        }
    }

    public Brush RebuildBrush(BrushType brushType)
    {
        switch (brushType)
        {
            default:
            case BrushType.Coloring:
                var cb = Instantiate(pfColorBrush);
                cb.transform.position = InputHelper.MouseWorldPositionIn2D;
                DataManager.Instance.RegisterRebuildBrush(cb);
                return cb;
            case BrushType.Line:
                var lb = Instantiate(pfLineBrush);
                lb.transform.position = InputHelper.MouseWorldPositionIn2D;
                DataManager.Instance.RegisterRebuildBrush(lb);
                return lb;
        }
    }

    public Connector NewConnector()
    {
        var connector = Instantiate(pfConnector);
        connector.transform.position = InputHelper.MouseWorldPositionIn2D;
        DataManager.Instance.RegisterNewDevice(connector);
        return connector;
    }

    private void OnControllerDestory(Controller controller)
    {
        commandReaderConsoleDictionary.Remove(controller);
    }

    int GetIndexFromIndicesArray()
    {
        for (int i = 0; i < commandReadersIndices.Length; i++)
        {
            if (commandReadersIndices[i] == 0)
            {
                commandReadersIndices[i] = i + 1;
                return i + 1;
            }
        }

        return 0;
    }

    void ResetHighlightingCommands()
    {
        foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            console.ResetAll();
        }
    }

    void RunNewLine(int commandLineIndex)
    {
        foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            console.ReadCommand(commandLineIndex);
        }
    }

    int GetCommandLineMaxIndex()
    {
        int maxIndex = -1;
        foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
        {

            if (console.MaxCommandIndex >= maxIndex)
            {
                maxIndex = console.MaxCommandIndex;
            }
        }
        return maxIndex;
    }
}

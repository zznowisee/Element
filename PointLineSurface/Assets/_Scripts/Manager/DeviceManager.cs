using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeviceManager : MonoBehaviour
{
    [SerializeField] Transform consolePanel;

    public static DeviceManager Instance { get; private set; }
    [Header("Ô¤ÖÆÌå")]
    [SerializeField] PointBrush pfColorBrush;
    [SerializeField] LineBrush pfLineBrush;
    [SerializeField] SurfaceBrush pfSurfaceBrush;
    [SerializeField] Device_Connector pfConnector;
    [SerializeField] Device_Marker pfMarker;
    [SerializeField] Device_Controller pfController;
    [SerializeField] Device_Extender pfExtender;
    [SerializeField] Console pfConsole;
    [SerializeField] Command pfCommand;
    [Header("Ö¸ÁîSO")]
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


    private int[] commandReadersIndices;

    void Awake()
    {
        Instance = this;
        commandReadersIndices = new int[15];
    }

    void Start()
    {
        UI_OperateScene.Instance.OnExitOperateScene += () => commandReadersIndices = new int[15];
    }

    public Command GetNewCommand(Transform parent) => Instantiate(pfCommand, parent);

    public void BuildDevices(Data_Solution solutionData_)
    {
        print("Switch to operating room");
        List<Data_Controller> controllerDatas = solutionData_.ControllerDatas;
        List<Data_Connector> connectorDatas = solutionData_.ConnectorDatas;
        //controller and console
        print("controllerDatas.Count : " + controllerDatas.Count);
        for (int i = 0; i < controllerDatas.Count; i++)
        {
            Device_Controller controller = Instantiate(pfController);
            Console console = Instantiate(pfConsole, consolePanel);

            int consoleIndex = controllerDatas[i].orderIndex;

            //Get CommandSO List
            Command[] rebuildCommands = new Command[controllerDatas[i].consoleData.commandTypes.Length];
            for (int j = 0; j < rebuildCommands.Length; j++)
            {
                if (controllerDatas[i].consoleData.commandTypes[j] == CommandType.Empty)
                    continue;

                CommandSO commandSO = GetCommandSOFromType(controllerDatas[i].consoleData.commandTypes[j]);
                if (commandSO != null)
                {
                    rebuildCommands[j] = Instantiate(pfCommand, UI_OperateScene.Instance.transform);
                    rebuildCommands[j].Setup(commandSO);
                }
            }

            //Rebuild Controller and Console with data
            controller.Rebuild(controllerDatas[i]);
            console.Rebuild(controller, controllerDatas[i].consoleData, rebuildCommands);
            // Sign index
            commandReadersIndices[consoleIndex - 1] = consoleIndex;
            // Sign Action

            ProcessManager.Instance.SignNewControllerWithConsole(new ControllerConsole(controller, console));
        }

        //connector
        for (int i = 0; i < connectorDatas.Count; i++)
        {
            Device_Connector connector = Instantiate(pfConnector);
            //bind data
            connector.Rebuild(connectorDatas[i]);
        }
    }

    public void ResetControllerIndices(Device device) => commandReadersIndices[device.GetComponent<Device_Controller>().Index - 1] = 0;

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

    public void GetNewController()
    {
        Device_Controller controller = Instantiate(pfController);
        Console console = Instantiate(pfConsole, consolePanel);
        ControllerConsole controllerConsole = new ControllerConsole(controller, console);

        controller.transform.position = InputHelper.MouseWorldPositionIn2D;
        controller.Init(GetIndexFromIndicesArray());

        console.Init(controller);

        UI_MouseManager.Instance.SetClickedBeforeDragObj(controller);
        ProcessManager.Instance.SignNewControllerWithConsole(controllerConsole);
        DataManager.Instance.SignNewDevice(controller);
    }

    public Device_Brush NewBrush(BrushType brushType, ColorType colorType)
    {
        switch (brushType)
        {
            default:
            case BrushType.Point:
                var cb = Instantiate(pfColorBrush);
                cb.transform.position = InputHelper.MouseWorldPositionIn2D;
                cb.Init(brushType, colorType);
                DataManager.Instance.SignNewDevice(cb);
                UI_MouseManager.Instance.SetClickedBeforeDragObj(cb);
                return cb;
            case BrushType.Line:
                var lb = Instantiate(pfLineBrush);
                lb.transform.position = InputHelper.MouseWorldPositionIn2D;
                lb.Init(brushType, colorType);
                DataManager.Instance.SignNewDevice(lb);
                UI_MouseManager.Instance.SetClickedBeforeDragObj(lb);
                return lb;
            case BrushType.Surface:
                var sb = Instantiate(pfSurfaceBrush);
                sb.transform.position = InputHelper.MouseWorldPositionIn2D;
                sb.Init(brushType, colorType);
                DataManager.Instance.SignNewDevice(sb);
                UI_MouseManager.Instance.SetClickedBeforeDragObj(sb);
                return sb;
        }
    }

    public Device_Brush RebuildBrush(BrushType brushType)
    {
        switch (brushType)
        {
            default:
            case BrushType.Point:
                var cb = Instantiate(pfColorBrush);
                cb.transform.position = InputHelper.MouseWorldPositionIn2D;
                return cb;
            case BrushType.Line:
                var lb = Instantiate(pfLineBrush);
                lb.transform.position = InputHelper.MouseWorldPositionIn2D;
                return lb;
            case BrushType.Surface:
                var sb = Instantiate(pfSurfaceBrush);
                sb.transform.position = InputHelper.MouseWorldPositionIn2D; ;
                return sb;  
        }
    }

    public Device_Connector NewConnector()
    {
        var connector = Instantiate(pfConnector);
        connector.transform.position = InputHelper.MouseWorldPositionIn2D;
        DataManager.Instance.SignNewDevice(connector);
        return connector;
    }

    public Device_Extender NewExtender()
    {
        var extender = Instantiate(pfExtender);
        extender.transform.position = InputHelper.MouseWorldPositionIn2D;
        DataManager.Instance.SignNewDevice(extender);
        return extender;
    }

    public Device_Marker NewMarker()
    {
        var marker = Instantiate(pfMarker);
        marker.transform.position = InputHelper.MouseWorldPositionIn2D;
        DataManager.Instance.SignNewDevice(marker);
        return marker;
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
}

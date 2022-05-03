using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public LevelData levelData;
    public SolutionData solutionData;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OperatingRoomUI.Instance.OnSwitchToMainScene += SwitchToMainScene;
        ProcessManager.Instance.OnLevelComplete += LevelComplete;
    }

    public void RegisterRebuildController(Controller controller) => controller.OnDestoryByPlayer += RemoveControllerData;
    public void RegisterRebuildConnector(Connector connector) => connector.OnDestoryByPlayer += RemoveConnectorData;
    public void RegisterRebuildBrush(Brush brush) => brush.OnDestoryByPlayer += RemoveBrushData;
    public void RegisterNewDevice(Device device)
    {
        switch (device.deviceType)
        {
            case DeviceType.Brush:
                Brush brush = device.GetComponent<Brush>();
                brush.OnDestoryByPlayer += RemoveBrushData;
                solutionData.brushDatas.Add(brush.data);
                break;
            case DeviceType.Connector:
                Connector connector = device.GetComponent<Connector>();
                connector.OnDestoryByPlayer += RemoveConnectorData;
                solutionData.connectorDatas.Add(connector.data);
                break;
            case DeviceType.Controller:
                Controller controller = device.GetComponent<Controller>();
                controller.OnDestoryByPlayer += RemoveControllerData;
                solutionData.controllerDatas.Add(controller.data);
                break;
        }
    }

    public void DeleteSolution(SolutionData data)
    {
        levelData.SolutionDatas.Remove(data);
    }

    void SwitchToMainScene()
    {
        solutionData = null;
    }

    void LevelComplete()
    {
        levelData.completed = true;
    }

    void GetBrushData(Brush brush)
    {
        brush.OnDestoryByPlayer += RemoveBrushData;
        solutionData.brushDatas.Add(brush.data);
    }

    void RemoveBrushData(Brush brush)
    {
        brush.OnDestoryByPlayer -= RemoveBrushData;
        solutionData.brushDatas.Remove(brush.data);
    }

    void GetControllerData(Controller controller)
    {
        controller.OnDestoryByPlayer += RemoveControllerData;
        solutionData.controllerDatas.Add(controller.data);
    }

    private void RemoveControllerData(Controller controller)
    {
        controller.OnDestoryByPlayer -= RemoveControllerData;
        solutionData.controllerDatas.Remove(controller.data);
        print("Remove Controller Data");
    }

    void GetConnectorData(Connector connector)
    {
        connector.OnDestoryByPlayer += RemoveConnectorData;
        solutionData.connectorDatas.Add(connector.data);
    }

    void RemoveConnectorData(Connector connector)
    {
        connector.OnDestoryByPlayer -= RemoveConnectorData;
        solutionData.connectorDatas.Remove(connector.data);
    }

    public void LoadLevelData(LevelData levelData_)
    {
        levelData = levelData_;
    }

    public void LoadSolution(SolutionData solutionData_)
    {
        solutionData = solutionData_;
    }
}

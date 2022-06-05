using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static void SaveGameData()
    {
        string saveName = "GameData";
        SerializationHelper.Save(saveName, GameArchiveData);
    }


    public static DataManager Instance { get; private set; }

    public static Data_GameArchive GameArchiveData { get; private set; }
    public static Data_Level LevelData;
    public static Data_Solution SolutionData;

    public List<LevelBuildDataSO> levelBuildDataSOsList;

    private void LoadGameArchiveData()
    {
        string path = Application.persistentDataPath + "/ver.02_saves/" + "GameData" + ".save";
        GameArchiveData = (Data_GameArchive)SerializationHelper.Load(path);
        if(GameArchiveData == null)
        {
            GameArchiveData = new Data_GameArchive();
            levelBuildDataSOsList.ForEach(levelBuildData => GameArchiveData.LevelDatas.Add(new Data_Level(levelBuildData)));
            SaveGameData();
        }
    }

    void Awake()
    {
        Instance = this;
        LoadGameArchiveData();
    }

    void Start()
    {
        ProcessManager.Instance.OnLevelComplete += LevelComplete;
    }

    public void RemoveDeviceData(Device device)
    {
        switch (device.deviceType)
        {
            default:
            case DeviceType.Connector:
                Device_Connector connector = device as Device_Connector;
                SolutionData.ConnectorDatas.Remove(connector.data);
                break;
            case DeviceType.Controller:
                Device_Controller controller = device as Device_Controller;
                SolutionData.ControllerDatas.Remove(controller.data);
                break;
            case DeviceType.Brush:
                Device_Brush brush = device as Device_Brush;
                SolutionData.BrushDatas.Remove(brush.data);
                break;
            case DeviceType.Extender:
                print("Remove Extender Data.");
                break;
            case DeviceType.Marker:
                print("Remove Marker Data.");
                Device_Marker marker = device as Device_Marker;

                break;
        }
    }
    public void SignNewDevice(Device device)
    {
        switch (device.deviceType)
        {
            case DeviceType.Brush:
                Device_Brush brush = device.GetComponent<Device_Brush>();
                SolutionData.BrushDatas.Add(brush.data);
                break;
            case DeviceType.Connector:
                Device_Connector connector = device.GetComponent<Device_Connector>();
                SolutionData.ConnectorDatas.Add(connector.data);
                break;
            case DeviceType.Controller:
                Device_Controller controller = device.GetComponent<Device_Controller>();
                SolutionData.ControllerDatas.Add(controller.data);
                break;
        }
    }

    public void DeleteSolutionData(Data_Solution data) => LevelData.SolutionDatas.Remove(data);

    void LevelComplete()
    {
        LevelData.completed = true;
        SolutionData.complete = true;
    }

    public void AddNewSandboxData(Data_Sandbox sandboxData_) => GameArchiveData.SandboxDatas.Insert(sandboxData_.sandboxIndex, sandboxData_);
    public void RemoveSandboxData(Data_Sandbox sandboxData_) => GameArchiveData.SandboxDatas.Remove(sandboxData_);
}

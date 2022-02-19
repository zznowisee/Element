using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameData
{
    private List<LevelData> levelDatas;
    public List<LevelData> LevelDatas
    {
        get
        {
            if(levelDatas == null)
            {
                levelDatas = new List<LevelData>();
            }
            return levelDatas;
        }
    }
}

[Serializable]
public class LevelData
{
    public string levelName;
    public int levelIndex;
    public bool completed;
    private List<SolutionData> solutionDatas;
    public List<SolutionData> SolutionDatas
    {
        get
        {
            if(solutionDatas == null)
            {
                solutionDatas = new List<SolutionData>();
            }
            return solutionDatas;
        }
    }

    public LevelData(string levelName_, int levelIndex_)
    {
        levelName = levelName_;
        levelIndex = levelIndex_;
        completed = false;
    }
}

[Serializable]
public class SolutionData
{
    public bool isSandBox;
    public bool complete;
    public int solutionIndex;

    public List<ControllerData> controllerDatas;
    public List<ConnectorData> connectorDatas;
    public List<BrushBtnData> brushBtnDatas;
    public List<BrushData> brushDatas;

    public SolutionData(int solutionIndex_, LevelBuildDataSO levelBuildData)
    {
        complete = false;
        solutionIndex = solutionIndex_;
        connectorDatas = new List<ConnectorData>();
        brushDatas = new List<BrushData>();
        controllerDatas = new List<ControllerData>();
        brushBtnDatas = new List<BrushBtnData>();

        foreach(var data in levelBuildData.brushBtnDatas)
        {
            brushBtnDatas.Add(new BrushBtnData()
            {
                brushType = data.brushType,
                colorType = data.colorSO.colorType,
                number = data.number
            });
        }
    }
}

[Serializable]
public class DeviceData
{
    public int cellIndex;
    public DeviceType deviceType;
}

[Serializable]
public class BrushData : DeviceData
{
    public ColorType colorType;
    public BrushType brushType;
}

[Serializable]
public class ConnectorData : DeviceData
{

}

[Serializable]
public class ControllerData : DeviceData
{
    public int index;
    public Direction direction;
    public ConsoleData consoleData;
}
[Serializable]
public class ConsoleData
{
    public CommandType[] commandTypes;
}

[Serializable]
public class BrushBtnData
{
    public int number;
    public ColorType colorType;
    public BrushType brushType;
}

[Serializable]
public enum ColorType
{
    Blue,
    Yellow,
    Red,
    White
}

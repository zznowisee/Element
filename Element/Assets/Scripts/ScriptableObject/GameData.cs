using System.Collections;
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

    public bool FinishedAllLevels()
    {
        for (int i = 0; i < levelDatas.Count; i++)
        {
            if(levelDatas[i].levelType != LevelType.Ex)
            {
                if (!levelDatas[i].completed)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

[Serializable]
public class LevelData
{
    public string levelName;
    public int levelIndex;
    public bool completed;
    public LevelType levelType;
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

    public LevelData(string levelName_, int levelIndex_, LevelType levelType_)
    {
        levelName = levelName_;
        levelIndex = levelIndex_;
        levelType = levelType_;
        completed = false;
    }
}

[Serializable]
public class SolutionData
{
    public bool hasInitialized;
    public bool complete;
    public int solutionIndex;
    public int controllerNumber;

    public List<ConnectorData> connectorDatas;
    public List<ControllerData> controllerDatas;
    public List<BrushBtnSolutionData> brushBtnDataSolutions;
    public SolutionData(int solutionIndex_)
    {
        hasInitialized = false;
        complete = false;
        solutionIndex = solutionIndex_;
        controllerNumber = 4;
        connectorDatas = new List<ConnectorData>();
        controllerDatas = new List<ControllerData>();
        brushBtnDataSolutions = new List<BrushBtnSolutionData>();
    }
}

[Serializable]
public class BrushData
{
    public BrushType type;
    public int cellIndex;
    public ColorType colorType;
}
[Serializable]
public class ConnectorData
{
    public int cellIndex;
}

[Serializable]
public class ControllerData
{
    public int cellIndex;
    public int consoleIndex;
    public Direction direction;
    public ConsoleData consoleData;
}
[Serializable]
public class ConsoleData
{
    public CommandType[] commandTypes;
}

[Serializable]
public class BrushBtnSolutionData
{
    //brush btn info :
    public ColorType colorType;
    public BrushType type;
    public int number;
    //brush info :
    public List<BrushData> brushDatas;

    public BrushBtnSolutionData(BrushBtnInitData initData)
    {
        brushDatas = new List<BrushData>();
        colorType = initData.colorSO.colorType;
        type = initData.type;
        number = initData.number;
    }
}

[Serializable]
public enum ColorType
{
    Blue,
    Yellow,
    Red,
    White
}

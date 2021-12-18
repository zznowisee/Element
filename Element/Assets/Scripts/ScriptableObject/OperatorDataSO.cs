using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorDataSO : ScriptableObject
{
    public bool hasInitialized;
    public int solutionIndex;
    public LevelDataSO level;
    public List<ConnectorData> connectorDatas;
    public List<ControllerData> controllerDatas;
    public List<ConsoleData> consoleDatas;
    public List<BrushBtnDataSolution> brushBtnDataSolutions;
    public void Setup()
    {
        connectorDatas = new List<ConnectorData>();
        controllerDatas = new List<ControllerData>();
        consoleDatas = new List<ConsoleData>();
        brushBtnDataSolutions = new List<BrushBtnDataSolution>();
    }
}

[System.Serializable]
public class BrushData
{
    public BrushType type;
    public int cellIndex;
    public ColorSO colorSO;
}
[System.Serializable]
public class ConnectorData
{
    public int cellIndex;
}
[System.Serializable]
public class ControllerData
{
    public int cellIndex;
    public int consoleIndex;
    public Direction direction;
    public ConsoleData consoleData;
}
[System.Serializable]
public class ConsoleData
{
    public int consoleIndex;
    public CommandSO[] commandSOs;
}
[System.Serializable]
public class BrushBtnDataSolution
{
    //brush btn info :
    public ColorSO colorSO;
    public BrushType type;
    public int number;
    //brush info :
    public List<BrushData> brushDatas;

    public BrushBtnDataSolution(BrushBtnDataInit initData)
    {
        brushDatas = new List<BrushData>();
        colorSO = initData.colorSO;
        type = initData.type;
        number = initData.number;
    }
}

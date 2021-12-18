using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorDataSO : ScriptableObject
{
    public bool hasInitialized;
    public int solutionIndex;
    public LevelDataSO level;
    public List<BrushData> brushDatas;
    public List<ConnectorData> connectorDatas;
    public List<ControllerData> controllerDatas;
    public List<ConsoleData> consoleDatas;
    public List<BrushBtnData> brushBtnDatas;
    public List<BrushBtnBrushData> brushBtnBrushDatas;
    public void Setup()
    {
        brushDatas = new List<BrushData>();
        connectorDatas = new List<ConnectorData>();
        controllerDatas = new List<ControllerData>();
        consoleDatas = new List<ConsoleData>();
        brushBtnDatas = new List<BrushBtnData>();
        brushBtnBrushDatas = new List<BrushBtnBrushData>();
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
}
[System.Serializable]
public class ConsoleData
{
    public int consoleIndex;
    public CommandSO[] commandSOs;
}
[System.Serializable]
public class BrushBtnBrushData
{
    public BrushBtnData brushBtnData;
    public BrushData brushData;
}

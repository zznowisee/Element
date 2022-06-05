using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Data_Device
{
    public Vector2Int index;
    public DeviceType type;
}

[Serializable]
public class Data_Brush : Data_Device
{
    public ColorType colorType;
    public BrushType brushType;
}

[Serializable]
public class Data_Connector : Data_Device
{
    public Direction direction;
}

[Serializable]
public class Data_Controller : Data_Device
{
    public int orderIndex;
    public Direction direction;
    public ConsoleData consoleData;
}

[Serializable]
public class Data_Extender : Data_Device
{
    public Direction direction;
}

[Serializable]
public class ConsoleData
{
    public CommandType[] commandTypes;
}

[Serializable]
public class Data_BrushBtn
{
    public int number;
    public ColorType colorType;
    public BrushType brushType;
    public Data_BrushBtn(Data_BrushBtn clone)
    {
        number = clone.number;
        colorType = clone.colorType;
        brushType = clone.brushType;
    }
}

[Serializable]
public class Data_LevelBuild
{
    public int markerLeftNumber;
    public int controllerLeftNumber;
    public int connectorLeftNumber;
    public List<Data_BrushBtn> BrushBtnDatas;
    public Data_LevelBuild(Data_LevelBuild levelBuildData_)
    {
        controllerLeftNumber = levelBuildData_.controllerLeftNumber;
        connectorLeftNumber = levelBuildData_.connectorLeftNumber;
        BrushBtnDatas = new List<Data_BrushBtn>();
        foreach (var clone in levelBuildData_.BrushBtnDatas)
        {
            BrushBtnDatas.Add(new Data_BrushBtn(clone));
        }
    }
}

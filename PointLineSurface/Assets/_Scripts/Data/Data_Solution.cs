using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_Solution
{
    public bool complete;
    public int solutionIndex;
    public List<Data_Controller> ControllerDatas;
    public List<Data_Connector> ConnectorDatas;
    public List<Data_Brush> BrushDatas;
    public Data_LevelBuild levelBuildData;

    public Data_Solution(int solutionIndex_, Data_Level levelData_)
    {
        complete = false;
        solutionIndex = solutionIndex_;
        ControllerDatas = new List<Data_Controller>();
        ConnectorDatas = new List<Data_Connector>();
        BrushDatas = new List<Data_Brush>();
        levelBuildData = new Data_LevelBuild(levelData_.levelInitData);
    }
}

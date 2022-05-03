using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelBuildDataSO : ScriptableObject
{
    public bool isSandbox;
    public new string name;
    public ProductData productData;
    public int levelIndex;
    public int developerCycle;
    public int controllerNum;
    public List<BrushBtnInitData> brushBtnDatas;
}



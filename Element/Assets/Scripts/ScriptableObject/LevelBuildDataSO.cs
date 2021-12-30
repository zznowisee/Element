using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelBuildDataSO : ScriptableObject
{
    public new string name;
    public LevelType levelType;
    public ProductData productData;
    public int levelIndex;
    public int controllerNum;
    public List<BrushBtnInitData> brushBtnDatas;
}



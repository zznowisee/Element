using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelDataSO : ScriptableObject
{
    public new string name;
    public LevelType levelType;
    public ProductData productData;

    public List<BrushBtnData> brushBtnDatas;
    public List<OperatorDataSO> operatorDataList;
}

public enum LevelType
{
    Tutorials,
    Middle,
    High
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelBuildDataSO : ScriptableObject
{
    public string levelName;
    public Chapter chapter;
    public int levelIndex;
    public int developerCycle;
    public Data_LevelBuild levelBuildData;
    public Data_Product productData;
}
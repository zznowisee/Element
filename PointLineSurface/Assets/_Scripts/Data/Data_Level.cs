using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_Level
{
    public string levelName;
    public int levelIndex;

    public Chapter chapter;

    public bool completed;
    // build datas
    public Data_ProductCell[] productCellDatas;
    // device datas
    public List<Data_Solution> SolutionDatas;
    public Data_LevelBuild levelInitData;
    public Data_Level(LevelBuildDataSO levelBuildDataSO_)
    {
        levelName = levelBuildDataSO_.levelName;
        levelIndex = levelBuildDataSO_.levelIndex;
        chapter = levelBuildDataSO_.chapter;
        completed = false;

        productCellDatas = new Data_ProductCell[levelBuildDataSO_.productData.productCellDatas.Length];

        for (int i = 0; i < levelBuildDataSO_.productData.productCellDatas.Length; i++)
        {
            var productCellData = levelBuildDataSO_.productData.productCellDatas[i];
            productCellDatas[i] = productCellData;
        }

        levelInitData = new Data_LevelBuild(levelBuildDataSO_.levelBuildData);

        SolutionDatas = new List<Data_Solution>();
    }
}

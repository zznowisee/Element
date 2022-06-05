using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data_Sandbox
{
    public int sandboxIndex;
    public List<Data_ProductCell> productCellDatas;

    public Data_Sandbox(int sandboxIndex_)
    {
        sandboxIndex = sandboxIndex_;
        productCellDatas = new List<Data_ProductCell>();
    }
}

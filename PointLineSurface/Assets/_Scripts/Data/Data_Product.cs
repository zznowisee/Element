using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Data_Product
{
    public Data_ProductCell[] productCellDatas;
}

[System.Serializable]
public class PointData
{
    public Vector2Int index;
    public PointData upPoint;
}
[System.Serializable]
public struct Data_ProductCell
{
    public int x, y;
    public bool hasLine;
    public bool hasPoint;
    public ColorType pointColorType;
    public List<Product_Line> lines;
}

[System.Serializable]
public struct Product_Line
{
    public Direction direction;
    public ColorType colorType;

    public static bool operator == (Product_Line a, Product_Line b)
    {
        return a.direction == b.direction && a.colorType == b.colorType;
    }

    public static bool operator !=(Product_Line a, Product_Line b)
    {
        return !(a == b);
    }
}

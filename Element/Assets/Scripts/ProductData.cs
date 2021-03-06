using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductData
{
    public LineData[] lines;
    public CellData[] cells;
}

[System.Serializable]
public class CellData
{
    public Vector2Int coord;
    public ColorSO color;
}

[System.Serializable]
public class LineData
{
    public Vector2Int pointA;
    public Vector2Int pointB;
    public ColorSO color;
}

[System.Serializable]
public class BrushBtnInitData
{
    public ColorSO colorSO;
    public BrushType type;
    public int number;
}

[System.Serializable]
public class PointData
{
    public Vector2Int index;
    public PointData upPoint;
}

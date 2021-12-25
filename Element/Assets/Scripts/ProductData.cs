using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductData
{
    public LineData[] lines;
    public CellData[] cells;
    public LineData_[] lineArray;
    public CellData_[] cellArray;
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
public class BrushBtnDataInit
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

[System.Serializable]
public class LineData_
{
    public ColorSO color;
    public PointData pointA;
    public PointData pointB;
}

[System.Serializable]
public class CellData_
{
    public ColorSO color;
    public PointData point;
}

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
    public ColorSO buildColor;
    public ColorSO drawColor;
}

[System.Serializable]
public class LineData
{
    public Vector2Int pointA;
    public Vector2Int pointB;
    public ColorSO buildColor;
    public ColorSO drawColor;
}

[System.Serializable]
public class BrushBtnData
{
    public ColorSO colorSO;
    public BrushType type;
    public int number;
}

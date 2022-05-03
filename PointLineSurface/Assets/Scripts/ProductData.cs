using UnityEngine;

[System.Serializable]
public class ProductData
{
    public LineCellData[] lines;
    public ColorCellData[] cells;
}

[System.Serializable]
public class ColorCellData
{
    public Vector2Int coord;
    public ColorSO color;
}

[System.Serializable]
public class LineCellData
{
    public Vector2Int coord;
    public Direction direction;
    public ColorSO color;
}

[System.Serializable]
public class BrushBtnInitData
{
    public ColorSO colorSO;
    public BrushType brushType;
    public int number;
}

[System.Serializable]
public class PointData
{
    public Vector2Int index;
    public PointData upPoint;
}

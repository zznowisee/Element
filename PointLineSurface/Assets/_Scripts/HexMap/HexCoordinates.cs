using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField] int x, y;
    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return -X - Y; } }

    public HexCoordinates(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public static HexCoordinates SetCoordinates(int x_, int y_)
    {
        //return new HexCoordinates(x_ - y_ / 2, y_);
        return new HexCoordinates(x_ , y_);
    }

    public Vector2Int GetValue()
    {
        return new Vector2Int(x, y);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public string LabelString()
    {
        return $"{X}\n{Y}\n{Z}";
    }

    #region Override Operator
    public static bool operator ==(HexCoordinates c1, HexCoordinates c2)
    {
        return c1.X == c2.X && c1.Y == c2.Y;
    }

    public static HexCoordinates operator + (HexCoordinates c1, HexCoordinates c2)
    {
        return new HexCoordinates(c1.x + c2.x, c1.y + c2.y);
    }

    public static HexCoordinates operator - (HexCoordinates c1, HexCoordinates c2)
    {
        return new HexCoordinates(c1.x - c2.x, c1.y - c2.y);
    }

    public static bool operator !=(HexCoordinates c1, HexCoordinates c2)
    {
        return !(c1 == c2);
    }
    #endregion
}

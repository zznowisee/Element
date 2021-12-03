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

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public string LabelString()
    {
        return $"{X}\n{Y}\n{Z}";
    }
}

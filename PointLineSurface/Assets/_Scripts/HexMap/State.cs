using UnityEngine;

[System.Serializable]
public enum Chapter
{
	Tutorial
}
public enum RotateDirection
{
	CW,
	CCW
}
public enum DeviceType
{
	Connector,
	Controller,
	Brush,
	Extender,
	Marker
}

public enum WarningType
{
	Collision,
	ReceiveTwoMoveCommands,
	ConnectedByTwo
}

[System.Serializable]
public enum EraseType
{
	FULL,
	DIRECTION
}

[System.Serializable]
public enum Direction 
{
	NE, E, SE, SW, W, NW
}

[System.Serializable]
public enum BrushType
{
	Point,
	Line,
	Surface
}

public enum BrushState
{
	PUTUP,
	PUTDOWN
}

[System.Serializable]
public enum ColorType
{
	NULL,
	// level one
	Blue,
	Yellow,
	Red,
	// level two
	Purple,
	Green,
	Orange
}

[System.Serializable]
public enum CommandType
{
    Empty = 0,
    Delay,
    PutDown,
    PutUp,
    ConnectorCW,
    ConnectorCCW,
    Connect,
    Split,
    Push,
    Pull,
    ControllerCCW,
    ControllerCW,
    Lock,
    Unlock
}

#region Direction Extension
public static class HexDirectionExtensions 
{

	public static Vector3Int GetValue(this Direction direction)
    {
        switch (direction)
        {
			default:
			case Direction.E: return new Vector3Int(1, 0, -1);
			case Direction.SE: return new Vector3Int(1, -1, 0);
			case Direction.SW: return new Vector3Int(0, -1, 1);
			case Direction.W: return new Vector3Int(-1, 0, 1);
			case Direction.NW: return new Vector3Int(-1, 1, 0);
			case Direction.NE: return new Vector3Int(0, 1, -1);
		}
    }

	public static Direction Opposite (this Direction direction) 
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static Direction Previous (this Direction direction) 
	{
		return direction == Direction.NE ? Direction.NW : (direction - 1);
	}

	public static Direction Next (this Direction direction)
	{
		return direction == Direction.NW ? Direction.NE : (direction + 1);
	}

	public static bool IsOpposite(this Direction d1, Direction d2)
    {
		return d1 == d2.Opposite();
    }

	public static HexCoordinates HexCoordinateValue(this Direction direction)
    {
        switch (direction)
        {
			default:
			case Direction.NW: return new HexCoordinates(-1,  1);
			case Direction.NE: return new HexCoordinates( 0,  1);
			case Direction.E:  return new HexCoordinates( 1,  0);
			case Direction.SE: return new HexCoordinates( 1, -1);
			case Direction.SW: return new HexCoordinates( 0, -1);
			case Direction.W:  return new HexCoordinates(-1,  0);
        }
    }
}
#endregion

#region ColorType Extension

public static class ColorTypeExtension
{
    public static ColorType Mix(ColorType type01, ColorType type02)
    {
        switch (type01)
        {
			default:
            case ColorType.Blue:
                switch (type02)
                {
                    default:
                    case ColorType.Blue: return ColorType.Blue;
                    case ColorType.Yellow: return ColorType.Green;
                    case ColorType.Red: return ColorType.Purple;
                    case ColorType.Purple: return ColorType.NULL;
                    case ColorType.Green: return ColorType.NULL;
                    case ColorType.Orange: return ColorType.NULL;
                }
            case ColorType.Yellow:
                switch (type02)
                {
                    default:
                    case ColorType.Yellow: return ColorType.Yellow;
                    case ColorType.Blue: return ColorType.Green;
                    case ColorType.Red: return ColorType.Orange;
                    case ColorType.Purple: return ColorType.NULL;
                    case ColorType.Green: return ColorType.NULL;
                    case ColorType.Orange: return ColorType.NULL;
                }
            case ColorType.Red:
                switch (type02)
                {
                    default:
                    case ColorType.Red: return ColorType.Red;
                    case ColorType.Blue: return ColorType.Purple;
                    case ColorType.Yellow: return ColorType.Orange;
                    case ColorType.Purple: return ColorType.NULL;
                    case ColorType.Green: return ColorType.NULL;
                    case ColorType.Orange: return ColorType.NULL;
                }
            case ColorType.Purple:
                return type02 == ColorType.Purple ? ColorType.Purple : ColorType.NULL;
            case ColorType.Green:
				return type02 == ColorType.Green ? ColorType.Green : ColorType.NULL;
            case ColorType.Orange:
				return type02 == ColorType.Orange ? ColorType.Orange : ColorType.NULL;
		}
    }
}
#endregion
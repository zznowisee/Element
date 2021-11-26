public enum Direction 
{
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions 
{

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

    public static Direction MoveDirection(this Direction from, Direction to)
    {
        switch (from)
        {
            case Direction.NE:
				if (to == Direction.E) return Direction.SE;
				else if (to == Direction.NW) return Direction.W;
                break;
            case Direction.E:
				if (to == Direction.SE) return Direction.SW;
				else if (to == Direction.NE) return Direction.NW;
                break;
            case Direction.SE:
				if (to == Direction.SW) return Direction.W;
				else if (to == Direction.E) return Direction.NE;
                break;
            case Direction.SW:
				if (to == Direction.W) return Direction.NW;
				else if (to == Direction.SE) return Direction.E;
                break;
            case Direction.W:
				if (to == Direction.NW) return Direction.NE;
				else if (to == Direction.SW) return Direction.SE;
                break;
            case Direction.NW:
				if (to == Direction.NE) return Direction.E;
				else if (to == Direction.W) return Direction.SW;
				break;
        }

		return Direction.NE;
    }

	public static Direction MoveByDirection(this Direction from, Direction dir)
    {
		switch (from)
		{
			case Direction.NE:
				if (dir == Direction.SE) return Direction.E;
				else if (dir == Direction.W) return Direction.NW;
                break;
            case Direction.E:
				if (dir == Direction.SW) return Direction.SE;
				else if (dir == Direction.NW) return Direction.NE;
                break;
            case Direction.SE:
				if (dir == Direction.W) return Direction.SW;
				else if (dir == Direction.NW) return Direction.E;
                break;
            case Direction.SW:
				if (dir == Direction.NW) return Direction.W;
				else if (dir == Direction.E) return Direction.SE;
                break;
            case Direction.W:
				if (dir == Direction.NE) return Direction.NW;
				else if (dir == Direction.SE) return Direction.SW;
                break;
            case Direction.NW:
				if (dir == Direction.E) return Direction.NE;
				else if (dir == Direction.SW) return Direction.W;
				break;
		}

		return Direction.NE;
	}
}
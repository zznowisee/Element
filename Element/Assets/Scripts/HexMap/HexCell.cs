using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    HexCellMesh hexMesh;
    public HexCoordinates hexCoordinates;
    [HideInInspector] public HexCell[] neighbors;

    Track track;
    public Brush brush;
    public Connecter connecter;
    public ConnecterBrushSlot connecterSlot;
    public void Setup(Vector3 position, Transform parent, HexCoordinates coordinates)
    {
        neighbors = new HexCell[6];
        hexMesh = GetComponentInChildren<HexCellMesh>();
        hexMesh.Init();

        transform.position = position;
        transform.parent = parent;
        hexCoordinates = coordinates;

        gameObject.name = hexCoordinates.ToString();
    }

    public Direction GetHexDirection(HexCell cell)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] == cell)
            {
                return (Direction)i;
            }
        }

        return Direction.E;
    }

    public HexCell GetNeighbor(Direction direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(Direction direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexCell PreviousCell(HexCell cell)
    {
        Direction direction = GetHexDirection(cell);
        return neighbors[(int)direction.Previous()];
    }

    public HexCell NextCell(HexCell cell)
    {
        Direction direction = GetHexDirection(cell);
        return neighbors[(int)direction.Next()];
    }

    public bool IsNeighbor(HexCell cell)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] == cell)
            {
                return true;
            }
        }
        return false;
    }
    
    public bool IsEmpty()
    {
        return track == null;
    }

    public void SetTrack(Track track_)
    {
        track = track_;
    }

    public void ClearTrack()
    {
        track = null;
    }

    public bool CanInitNewBrush()
    {
        return brush == null;
    }

    public bool CanInitNewConnecterSlot(out Connecter connecter_, out Direction direction_)
    {
        if(connecterSlot == null && connecter == null)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if(neighbors[i].connecter != null)
                {
                    connecter_ = neighbors[i].connecter;
                    direction_ = ((Direction)i).Opposite();
                    return true;
                }
            }
        }
        connecter_ = null;
        direction_ = 0;
        return false;
    }

    public bool CanEnterConnecterOrSlot()
    {
        return connecter == null && connecterSlot == null;
    }

    public void Coloring(Color col)
    {
        hexMesh.Coloring(col);
        ProcessSystem.Instance.colorCells.Add(this);
    }

    public void ClearColor()
    {
        hexMesh.ResetColor();
    }
}

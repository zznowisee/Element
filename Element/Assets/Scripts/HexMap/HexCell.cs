using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexCell : MonoBehaviour
{
    HexCellMesh hexMesh;
    public HexCoordinates hexCoordinates;
    [HideInInspector] public HexCell[] neighbors;
    [SerializeField] TextMeshPro indexText;

    Track track;
    public Brush brush;
    public Connector connector;
    public Controller controller;
    public ICommandReciever reciever;
    public void Setup(Vector3 position, Transform parent, HexCoordinates coordinates)
    {
        neighbors = new HexCell[6];
        hexMesh = GetComponentInChildren<HexCellMesh>();
        hexMesh.Init();

        transform.position = position;
        transform.parent = parent;
        hexCoordinates = coordinates;

        gameObject.name = hexCoordinates.ToString();
        bool debug = false;
        if (debug)
        {
            indexText.text = $"{hexCoordinates.X}\n{hexCoordinates.Y}\n{hexCoordinates.Z}";
        }
        else
        {
            indexText.text = "";
        }
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

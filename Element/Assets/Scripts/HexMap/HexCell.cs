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

    public bool beColoring = false;

    Track track;
    public Brush brush;
    public Connector connector;
    public Controller controller;
    public ICommandReciever reciever;
    public Transform patternLineHolder;
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

    public Direction GetTwoNeighborsMoveDirection(HexCell from, HexCell to)
    {
        Direction fromDir = GetHexDirection(from);
        Direction toDir = GetHexDirection(to);
        return fromDir.MoveDirection(toDir);
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
    
    public bool IsEmpty()
    {
        return brush == null && connector == null && controller == null;
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

    public void PaintingWithColor(Color col, int sortingOrder)
    {
        beColoring = true;
        hexMesh.Coloring(col);
        hexMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        ProcessSystem.Instance.recordCells.Add(this);
    }

    public void ResetCell()
    {
        hexMesh.ResetColor();
        for (int i = 0; i < patternLineHolder.childCount; i++)
        {
            Destroy(patternLineHolder.GetChild(i).gameObject);
        }
        beColoring = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    HexCellMesh hexMesh;
    public HexCoordinates hexCoordinates;

    [SerializeField]
    HexCell[] neighbors;

    List<WayPoint> wayPoints;

    public FullColorBrush brush;
    public void Setup(Vector3 position, Transform parent, HexCoordinates coordinates)
    {
        neighbors = new HexCell[6];
        hexMesh = GetComponentInChildren<HexCellMesh>();
        wayPoints = new List<WayPoint>();
        hexMesh.Init();

        transform.position = position;
        transform.parent = parent;
        hexCoordinates = coordinates;

        gameObject.name = hexCoordinates.ToString();
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
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
    
    public bool CanInitNewWire()
    {
        return wayPoints.Count == 0;
    }

    public WayPoint GetEndOfWireWayPoint()
    {
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if(wayPoints[i].GetWirePointType() == WirePointType.START ||
                wayPoints[i].GetWirePointType() == WirePointType.END)
            {
                return wayPoints[i];
            }
        }
        return null;
    }

    public void AddNewWayPoint(WayPoint wayPoint)
    {
        wayPoints.Add(wayPoint);
    }

    public void RemoveWayPoint(WayPoint wayPoint)
    {
        wayPoints.Remove(wayPoint);
    }

    public void SetBrush(FullColorBrush brush_)
    {
        brush = brush_;
    }

    public void ClearBrush()
    {
        brush = null;
    }

    public void Coloring(Color col)
    {
        hexMesh.Coloring(col);
    }
}

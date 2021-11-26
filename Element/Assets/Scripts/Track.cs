using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrackDirection
{
    PLUS,
    MINUS
}

public class Track : MonoBehaviour, IMouseDrag
{
    [SerializeField] float wireWidth = 2f;
    [SerializeField] TrackControlPoint pfControlPoint;
    [SerializeField] TrackControlPoint endControlPoint;
    [SerializeField] TrackControlPoint startControlPoint;
    TrackMesh trackMesh;
    List<HexCell> cells;

    public void Setup(HexCell cell_)
    {
        trackMesh = GetComponent<TrackMesh>();

        cells = new List<HexCell>() { cell_ };
        transform.position = cell_.transform.position;

        trackMesh.Setup(wireWidth);
        endControlPoint.Setup(this, TrackControlPointType.END, cell_);
    }

    public void StartDragging()
    {
        
    }

    public void EnterCell(HexCell cell, TrackControlPointType type)
    {
        if(IsPreviousCell(type, cell))
        {
            trackMesh.EnterPreviousCell(type);
            if(type == TrackControlPointType.START)
            {
                cells[0].ClearTrack();
                cells.RemoveAt(0);
            }
            else if(type == TrackControlPointType.END)
            {
                cells[cells.Count - 1].ClearTrack();
                cells.RemoveAt(cells.Count - 1);
            }
        }
        else
        {
            cell.SetTrack(this);
            Vector3 cellPosition = cell.transform.position;
            trackMesh.EnterNewCell(cellPosition, type);
            if (cells.Count == 1)
            {
                startControlPoint = Instantiate(pfControlPoint, cells[0].transform.position, Quaternion.identity, transform);
                startControlPoint.Setup(this, TrackControlPointType.START, cells[0]);
            }

            if(type == TrackControlPointType.START)
            {
                cells.Insert(0, cell);
            }
            else if(type == TrackControlPointType.END)
            {
                cells.Add(cell);
            }
        }
    }

    public bool IsPreviousCell(TrackControlPointType type, HexCell cell)
    {
        if(cells.Count < 2)
        {
            return false;
        }

        if(type == TrackControlPointType.START)
        {
            return cell == cells[1];
        }
        else
        {
            return cell == cells[cells.Count - 2];
        }
    }

    public HexCell GetNextCell(HexCell current, TrackDirection direction)
    {
        int currentIndex = cells.IndexOf(current);
        if(currentIndex == 0 || currentIndex == cells.Count - 1)
        {
            return null;
        }

        switch (direction)
        {
            case TrackDirection.MINUS:
                return cells[currentIndex + 1];
            case TrackDirection.PLUS:
                return cells[currentIndex - 1];
        }

        return null;
    }

    public void EndDragging()
    {
        
    }
}

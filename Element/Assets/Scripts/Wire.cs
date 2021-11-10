using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] float wireWidth = 2f;
    WireMesh wireMesh;
    [SerializeField] WayPoint pfWayPoint;
    List<HexCell> cells;
    List<WayPoint> wayPoints;
    Transform wayPointsHolder;
    HexCell trackingCell;
    WayPoint trackingWayPoint;

    HexCell LastCell { get { return cells[cells.Count - 1]; } }
    HexCell FirstCell { get { return cells[0]; } }

    public void Init(HexCell cell_)
    {
        cells = new List<HexCell>();
        wayPoints = new List<WayPoint>();
        wireMesh = GetComponent<WireMesh>();
        wayPointsHolder = transform.Find("wayPointsHolder");

        wireMesh.Init(wireWidth);

        WayPoint wayPoint = Instantiate(pfWayPoint, cell_.transform.position, Quaternion.identity, wayPointsHolder);
        wayPoint.Init(this, WirePointType.START, $"{cell_.gameObject.name}_WayPoint");
        trackingWayPoint = wayPoint;
        trackingCell = cell_;
        cells.Add(cell_);
        wayPoints.Add(wayPoint);

        cell_.AddNewWayPoint(wayPoint);
    }

    public void SetToNewCell(HexCell newCell)
    {
        WayPoint wayPoint = Instantiate(pfWayPoint, newCell.transform.position, Quaternion.identity, wayPointsHolder);
        string wayPointName = $"{newCell.gameObject.name}_WayPoint";
        if (trackingCell == LastCell)
        {
            wireMesh.OnEnterNewCell(newCell.transform.position, WirePointType.END);
            wayPoint.Init(this, WirePointType.END, wayPointName);
            cells.Add(newCell);
            wayPoints.Add(wayPoint);
        }
        else if(trackingCell == FirstCell)
        {
            wireMesh.OnEnterNewCell(newCell.transform.position, WirePointType.START);
            wayPoint.Init(this, WirePointType.START, wayPointName);
            cells.Insert(0, newCell);
            wayPoints.Insert(0, wayPoint);
        }
        if (wayPoints.Count > 2)
        {
            trackingWayPoint.SetToMiddle();
        }
        trackingCell = newCell;
        trackingWayPoint = wayPoint;
        newCell.AddNewWayPoint(wayPoint);
    }

    public void RemovePreviousCell()
    {
        if (trackingCell == LastCell)
        {
            RemoveLastWayPoint();
        }
        else if(trackingCell == FirstCell)
        {
            RemoveFirstWayPoint();
        }
    }
    #region GetEndPosition
    /*    Vector3 GetEndPosition()
        {
            Vector3 mousePos = InputHelper.MouseWorldPositionIn2D;
            Vector3 dir = (mousePos - cells[cells.Count - 1].transform.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            dir = InputHelper.GetDirectionFromAngleInHex(angle);
            Vector3 endPosition = InputHelper.GetWireEndPositionFromMouse(transform.position, dir);
            return endPosition;
        }*/
    #endregion

    void RemoveLastWayPoint()
    {
        wireMesh.OnEnterPreviousCell(WirePointType.END);
        cells[cells.Count - 1].RemoveWayPoint(wayPoints[wayPoints.Count - 1]);

        cells.RemoveAt(cells.Count - 1);
        Destroy(wayPoints[wayPoints.Count - 1].gameObject);
        wayPoints.RemoveAt(wayPoints.Count - 1);

        trackingWayPoint = wayPoints[wayPoints.Count - 1];
        trackingWayPoint.SetToEndOfLine(WirePointType.END);
        trackingCell = cells[cells.Count - 1];
    }

    void RemoveFirstWayPoint()
    {
        wireMesh.OnEnterPreviousCell(WirePointType.START);
        cells[0].RemoveWayPoint(wayPoints[0]);

        cells.RemoveAt(0);
        Destroy(wayPoints[0].gameObject);
        wayPoints.RemoveAt(0);

        trackingWayPoint = wayPoints[0];
        trackingWayPoint.SetToEndOfLine(WirePointType.START);
        trackingCell = cells[0];
    }

    public bool IsLastCell(HexCell cell)
    {
        if(cells.Count < 2)
        {
            return false;
        }

        if(trackingCell == FirstCell)
        {
            return cell == cells[1];
        }
        else if(trackingCell == LastCell)
        {
            return cell == cells[cells.Count - 2];
        }
        return false;
    }

    public void Finish()
    {
        if(cells.Count == 1)
        {
            cells[0].RemoveWayPoint(wayPoints[0]);
            Destroy(gameObject);
        }
        trackingWayPoint = null;
    }

    public void BeSelected(WayPoint wayPoint, HexCell cell)
    {
        trackingWayPoint = wayPoint;
        trackingCell = cell;
    }
}

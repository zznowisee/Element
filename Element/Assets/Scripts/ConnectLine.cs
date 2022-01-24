using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectLine : MonoBehaviour
{
    public ConnectableDevice start;
    public ConnectableDevice end;

    public LineRenderer lineRenderer;

    public void Setup(ConnectableDevice start_, ConnectableDevice end_)
    {
        start = start_;
        end = end_;
        lineRenderer.SetPosition(0, start.transform.position - transform.position);
        lineRenderer.SetPosition(1, end.transform.position - transform.position);
    }

    public void UpdatePosition()
    {
        lineRenderer.SetPosition(0, start.transform.position - transform.position);
        lineRenderer.SetPosition(1, end.transform.position - transform.position);
    }
}

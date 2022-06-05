using UnityEngine;

public class ConnectLine : MonoBehaviour
{
    ConnectableDevice start;
    Device_Connector end;
    LineRenderer lineRenderer;

    public void Setup(ConnectableDevice start_, Device_Connector end_)
    {
        lineRenderer = GetComponent<LineRenderer>();
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine currentDrawingLine;

    public override event Action<Vector3, WarningType> OnWarning;
    public override IEnumerator MoveToTarget(Connector connector, HexCell target, Action callback)
    {
        yield return null;
        cell.brush = null;
        if (CanPaint())
        {
            StartPainting();
        }

        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.SetPosition(1, connector.transform.position - transform.position);
            if (currentDrawingLine != null)
            {
                currentDrawingLine.Painting(transform.position);
            }

            yield return null;
        }

        cell = target;
        cell.brush = this;
        FinishPainting();
        callback?.Invoke();
    }

    void StartPainting()
    {
        currentDrawingLine = Instantiate(pfPatternLine);
        currentDrawingLine.Setup(cell, colorSO.color, ProcessSystem.Instance.commandLineIndex);
        ProcessSystem.Instance.recordCells.Add(cell);
    }

    void FinishPainting()
    {
        if(currentDrawingLine != null)
        {
            cell.beColoring = true;
            currentDrawingLine.endCell = cell;
            ProcessSystem.Instance.recordCells.Add(cell);
            currentDrawingLine = null;
        }
    }

    bool CanPaint()
    {
        return putdown;
    }

    public override void ConnectWithConnector(Connector connector_)
    {
        StartCoroutine(ConnectConnector(connector_));
    }

    IEnumerator ConnectConnector(Connector connector_)
    {
        yield return null;
        if (connector == null)
        {
            connector = connector_;
            connector_.OnMoveActionStart += Connector_OnMoveActionStart;
            connector_.OnRotateActionStart += Connector_OnRotateActionStart;
            connector_.OnSleepActionStart += Connector_OnSleepActionStart;

            connectLine = Instantiate(pfConnectLine, transform);
            connectLine.SetPosition(0, cell.transform.position - transform.position);
            connectLine.SetPosition(1, connector_.transform.position - transform.position);
            StartCoroutine(Sleep());
        }
        else
        {
            if (connector != connector_)
            {
                OnWarning?.Invoke(transform.position, WarningType.BrushConnectedByTwoConnectors);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (!ProcessSystem.Instance.CanOperate())
            {
                print("Enter");
                OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }
}

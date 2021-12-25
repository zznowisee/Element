using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine currentDrawingLine;

    public override event Action<Vector3, WarningType> OnWarning;
    public event Action<HexCell, HexCell, ColorSO> OnDrawingLine;
    public override IEnumerator MoveToTarget(Action controllerCallback, Action<Action> connectorCallback, Connector connector, HexCell target)
    {
        yield return null;
        cell.currentObject = null;
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
        cell.currentObject = gameObject;
        FinishPainting();
        connectorCallback?.Invoke(controllerCallback);
    }
    public IEnumerator ConnectConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_)
    {
        yield return null;
        if (connector == null)
        {
            connector = connector_;
            connector_.OnMoveActionStart += Connector_OnMoveActionStart;
            connector_.OnRotateActionStart += Connector_OnRotateActionStart;

            connectLine = Instantiate(pfConnectLine, transform);
            connectLine.SetPosition(0, cell.transform.position - transform.position);
            connectLine.SetPosition(1, connector_.transform.position - transform.position);
        }
        else
        {
            if (connector != connector_)
            {
                OnWarning?.Invoke(transform.position, WarningType.BrushConnectedByTwoConnectors);
            }
        }
    }
    void StartPainting()
    {
        cell.beColoring = true;
        currentDrawingLine = Instantiate(pfPatternLine);
        currentDrawingLine.Setup(cell, brushData.colorSO.drawColor, ProcessSystem.Instance.commandLineIndex);
        ProcessSystem.Instance.recordCells.Add(cell);
    }

    void FinishPainting()
    {
        if(currentDrawingLine != null)
        {

            cell.beColoring = true;
            currentDrawingLine.endCell = cell;
            ProcessSystem.Instance.recordCells.Add(cell);

            OnDrawingLine?.Invoke(currentDrawingLine.startCell, currentDrawingLine.endCell, brushData.colorSO);

            currentDrawingLine = null;
        }
    }

    bool CanPaint()
    {
        return putdown;
    }

    public override void ConnectWithConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_)
    {
        StartCoroutine(ConnectConnector(callback, secondLevelCallback, connector_));
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

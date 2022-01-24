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
    public override IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
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
            percent += Time.deltaTime / ProcessSystem.Instance.defaultExecuteTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            if (currentDrawingLine != null)
            {
                currentDrawingLine.Painting(transform.position);
            }

            yield return null;
        }

        cell = target;
        cell.currentObject = gameObject;
        FinishPainting();
        //connectorCallback?.Invoke(controllerCallback);
    }
    public IEnumerator ConnectConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_)
    {
        yield return null;
        if (connector == null)
        {
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
        currentDrawingLine.Setup(cell, brushBtn.colorSO.drawColor, ProcessSystem.Instance.commandLineIndex);
        ProcessSystem.Instance.recordCells.Add(cell);
    }

    void FinishPainting()
    {
        if(currentDrawingLine != null)
        {

            cell.beColoring = true;
            currentDrawingLine.endCell = cell;
            ProcessSystem.Instance.recordCells.Add(cell);

            OnDrawingLine?.Invoke(currentDrawingLine.startCell, currentDrawingLine.endCell, brushBtn.colorSO);

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

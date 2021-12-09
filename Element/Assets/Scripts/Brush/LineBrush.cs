using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine currentDrawingLine;

    public override event Action<HexCell, string> OnWarning;
    public override IEnumerator MoveToTarget(HexCell target, Action callback)
    {
        cell.brush = null;
        if (CanPainting())
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

        if (target.IsEmpty())
        {
            cell = target;
            cell.brush = this;
        }
        else
        {
            OnWarning?.Invoke(target, "Error#00!\nTwo devices enter one unit at the same time!");
        }

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
            currentDrawingLine = null;
        }
    }

    bool CanPainting()
    {
        return putdown;
    }
}

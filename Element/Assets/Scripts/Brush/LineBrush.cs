using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine currentDrawingLine;
    public override IEnumerator MoveToTarget(HexCell target, Action callback)
    {
        cell.brush = null;
        StartPainting();

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

        FinishPainting();

        cell = target;
        cell.brush = this;
        callback?.Invoke();
    }

    void StartPainting()
    {
        if (putdown)
        {
            currentDrawingLine = Instantiate(pfPatternLine);
            currentDrawingLine.Setup(cell, colorSO.color);
        }
    }

    void FinishPainting()
    {
        cell.PaintingWithLine(currentDrawingLine);
        currentDrawingLine = null;
    }
}

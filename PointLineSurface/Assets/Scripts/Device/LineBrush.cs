using System;
using System.Collections;
using UnityEngine;

public class LineBrush : Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine currentDrawingLine;
    public event Action<HexCell, HexCell, ColorType> OnDrawingLine;

    public override IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
    {
        yield return null;
        cell.currentObject = null;
        if (brushState == BrushState.PUTDOWN)
        {
            StartPainting();
        }

        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.UpdatePosition();
            if (currentDrawingLine != null)
            {
                currentDrawingLine.Painting(transform.position);
            }

            yield return null;
        }

        cell = target;
        cell.currentObject = gameObject;
        FinishPainting();
        callback?.Invoke(releaserCallback);
    }

    void StartPainting()
    {
        cell.beColoring = true;
        currentDrawingLine = Instantiate(pfPatternLine);
        currentDrawingLine.Setup(cell, drawCol, ProcessManager.Instance.LineIndex);
        ProcessManager.Instance.recordCells.Add(cell);
    }

    void FinishPainting()
    {
        if(currentDrawingLine != null)
        {
            cell.beColoring = true;
            currentDrawingLine.endCell = cell;
            ProcessManager.Instance.recordCells.Add(cell);

            OnDrawingLine?.Invoke(currentDrawingLine.startCell, currentDrawingLine.endCell, data.colorType);

            currentDrawingLine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null)
        {
            if (!ProcessManager.Instance.CanOperate())
            {
                print("Enter");
                //OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }
}

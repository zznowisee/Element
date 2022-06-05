using System;
using System.Collections;
using UnityEngine;

public class LineBrush : Device_Brush
{
    [SerializeField] PatternLine pfPatternLine;
    PatternLine startLine;
    PatternLine endLine;
    public event Action<HexCell, HexCell, ColorType> OnDrawingLine;

    public override IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
    {
        yield return null;
        cell.currentObject = null;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        Vector3 middlePosition = (startPosition + endPosition) / 2f;

        if (brushState == BrushState.PUTDOWN)
        {
            startLine = Instantiate(pfPatternLine);
            endLine = Instantiate(pfPatternLine);
            startLine.Setup(cell, startPosition, drawCol, ProcessManager.Instance.LineIndex, PatternLine.PatternLineType.START);
        }

        float percent = 0f;

        while (percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.UpdatePosition();

            if (startLine)
            {
                if(percent < 0.5f)
                {
                    startLine.Painting(transform.position);
                }
                else
                {
                    startLine.Painting(middlePosition);
                    startLine = null;
                    endLine.Setup(target, middlePosition, drawCol, ProcessManager.Instance.LineIndex, PatternLine.PatternLineType.END);
                }
            }

            if (!startLine && endLine)
            {
                if(percent < 1f)
                {
                    endLine.Painting(transform.position);
                }
                else
                {
                    endLine.Painting(endPosition);
                    endLine = null;
                }
            }

            yield return null;
        }
        if (brushState == BrushState.PUTDOWN) CheckManager.Instance.LineBrushDraw(cell, target, data.colorType);
        cell = target;
        cell.currentObject = gameObject;
        callback?.Invoke(releaserCallback);
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

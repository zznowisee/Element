using System.Collections;
using UnityEngine;
using System;

public class ColorBrush : Brush
{
    public event Action<HexCell, ColorType> OnColoringCell;

    public override IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
    {
        yield return null;
        cell.currentObject = null;
        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.UpdatePosition();
            yield return null;
        }

        cell = target;
        cell.currentObject = gameObject;
        TryPainiting();
        callback?.Invoke(releaserCallback);
    }

    void TryPainiting()
    {
        if (brushState == BrushState.PUTDOWN)
        {
            cell.PaintingWithColor(drawCol, ProcessManager.Instance.LineIndex);
            OnColoringCell?.Invoke(cell, data.colorType);
        }
    }

    public override void PutDownUp(Action releaserCallback, Action<Action> recieverCallback, BrushState state, float executeTime)
    {
        base.PutDownUp(releaserCallback, recieverCallback, state, executeTime);
        TryPainiting();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other != null)
        {
            if (!ProcessManager.Instance.CanOperate())
            {
                print("Enter");
                //OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }
}
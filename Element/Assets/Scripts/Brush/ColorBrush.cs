using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorBrush : Brush
{
    public override event Action<HexCell, string> OnWarning;
    public override IEnumerator MoveToTarget(HexCell target, Action callback)
    {
        cell.brush = null;

        float percent = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / ProcessSystem.Instance.commandDurationTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(startPosition, endPosition, percent);
            connectLine.SetPosition(1, connector.transform.position - transform.position);
            yield return null;
        }

        if(target.IsEmpty())
        {
            cell = target;
            cell.brush = this;
            TryPainiting();
        }
        else
        {
            OnWarning?.Invoke(target, "Error#00!\nTwo devices enter one unit at the same time!");
        }

        callback?.Invoke();
    }

    public void TryPainiting()
    {
        if (putdown)
        {
            cell.PaintingWithColor(colorSO.color);
        }
    }

    public override void PutDownUp(bool coloring_)
    {
        base.PutDownUp(coloring_);
        TryPainiting();
    }
}
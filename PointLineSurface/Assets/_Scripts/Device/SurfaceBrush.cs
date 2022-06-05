using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceBrush : Device_Brush
{
    private void Awake()
    {
        deviceType = DeviceType.Brush;
        brushType = BrushType.Surface;
    }

    public override IEnumerator MoveToTarget(Action releaserCallback, Action<Action> callback, HexCell target, float executeTime)
    {
        yield return null;

        cell.currentObject = null;

        float percent = 0f;
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;
        while (percent < 1f)
        {
            percent += Time.deltaTime / executeTime;
            percent = Mathf.Clamp01(percent);
            transform.position = Vector3.Lerp(start, end, percent);
            connectLine.UpdatePosition();
            yield return null;
        }
        cell = target;
        cell.currentObject = gameObject;
        if(brushState == BrushState.PUTDOWN)
        {
            Painting();
        }

        callback?.Invoke(releaserCallback);
    }

    private void Painting()
    {
        print("Surface Painting.");
        for (int i = 0; i < cell.neighbors.Length; i++)
        {
            if(cell.neighbors[i].currentObject != null)
            {
                if (cell.neighbors[i].currentObject.GetComponent<Device>().deviceType != DeviceType.Brush)
                {
                    continue;
                }
            }
            cell.neighbors[i].PaintingWithColor(drawCol, ProcessManager.Instance.LineIndex);
            print("Paint neighbors");
        }
        cell.PaintingWithColor(drawCol, ProcessManager.Instance.LineIndex);
    }

    public override void Putdown(Action releaserCallback, Action<Action> recieverCallback, float executeTime)
    {
        base.Putdown(releaserCallback, recieverCallback, executeTime);
        Painting();
    }
}

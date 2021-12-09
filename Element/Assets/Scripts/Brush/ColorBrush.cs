using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorBrush : Brush
{
    public override event Action<HexCell, string> OnWarning;
    public override IEnumerator MoveToTarget(Connector connector, HexCell target, Action callback)
    {
        yield return null;
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

    void TryPainiting()
    {
        if (putdown)
        {
            cell.PaintingWithColor(colorSO.color, ProcessSystem.Instance.commandLineIndex);
        }
    }

    public override void PutDownUp(bool coloring_)
    {
        base.PutDownUp(coloring_);
        TryPainiting();
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
                OnWarning?.Invoke(cell, "Error#04!\nThe brush is connected by two connectors at the same time!");
            }
        }
    }

    public override void ConnectWithConnector(Connector connector_)
    {
        StartCoroutine(ConnectConnector(connector_));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ColorBrush : Brush
{
    public override event Action<Vector3, WarningType> OnWarning;
    public event Action<HexCell, ColorSO> OnColoringCell;
    public override IEnumerator MoveToTarget(Action callback, Action<Action> secondLevelCallback, Connector connector, HexCell target)
    {
        yield return null;
        cell.currentObject = null;
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

        cell = target;
        cell.currentObject = gameObject;
        TryPainiting();

        secondLevelCallback?.Invoke(callback);
    }

    void TryPainiting()
    {
        if (putdown)
        {
            cell.PaintingWithColor(brushData.colorSO.drawColor, ProcessSystem.Instance.commandLineIndex);
            OnColoringCell?.Invoke(cell, brushData.colorSO);
        }
    }

    public override void PutDownUp(Action callback, Action<Action> secondLevelCallback, bool coloring_)
    {
        base.PutDownUp(callback, secondLevelCallback, coloring_);
        TryPainiting();
    }

    IEnumerator ConnectConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_)
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
            StartCoroutine(Sleep(callback, secondLevelCallback));
        }
        else
        {
            if (connector != connector_)
            {
                OnWarning?.Invoke(transform.position, WarningType.BrushConnectedByTwoConnectors);
            }
        }
    }

    public override void ConnectWithConnector(Action callback, Action<Action> secondLevelCallback, Connector connector_)
    {
        StartCoroutine(ConnectConnector(callback, secondLevelCallback, connector_));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other != null)
        {
            if (!ProcessSystem.Instance.CanOperate())
            {
                print("Enter");
                OnWarning?.Invoke(transform.position, WarningType.Collision);
            }
        }
    }
}
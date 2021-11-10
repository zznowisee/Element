using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ControlPoint : MonoBehaviour, IMouseDrag
{
    public int index;
    public WayPoint wayPoint;
    CodeConsole codeConsole;

    public event Action<ControlPoint> OnDestory;
    [HideInInspector] public bool isReady;

    void Awake()
    {
        isReady = false;    
    }

    public void SetIndex(int index_)
    {
        index = index_;
        GetComponentInChildren<TextMeshPro>().text = index.ToString();
    }

    public void Setup(WayPoint wayPoint_)
    {
        wayPoint = wayPoint_;
        transform.position = wayPoint.transform.position;
        wayPoint.SetControlPoint(this);
        isReady = true;
    }

    public void StartDragging()
    {
        wayPoint.ClearControlPoint();
        wayPoint = null;
        BuildSystem.Instance.SetCurrentTrackingControlPoint(this);
    }

    public void Delete()
    {
        OnDestory?.Invoke(this);
        Destroy(gameObject);
    }
}

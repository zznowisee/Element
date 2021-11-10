using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullColorBrush : MonoBehaviour, IMouseDrag
{
    [HideInInspector] public HexCell cell;

    public ColorSO color;
    MeshRenderer meshRenderer;
    public void StartDragging()
    {
        cell.ClearBrush();
        cell = null;
        BuildSystem.Instance.SetCurrentTrackingBrush(this);
    }

    public void Setup(HexCell initCell)
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        initCell.SetBrush(this);
        transform.position = initCell.transform.position;
        cell = initCell;
    }

    public void SetColorSO(ColorSO color_)
    {
        color = color_;
        meshRenderer.material.color = color.color;
    }

    public void ClearColorSO()
    {
        color = null;
    }
}

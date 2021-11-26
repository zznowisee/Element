using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Brush : MonoBehaviour, IMouseDrag
{
    public HexCell cell;
    public ConnecterBrushSlot slot;

    public ColorSO colorSO;
    MeshRenderer meshRenderer;

    HexCell recorderCell;

    public void StartDragging()
    {
        cell.brush = null;
        cell = null;

        if(slot != null)
        {
            slot.brush = null;
            slot = null;
        }

        BuildSystem.Instance.SetCurrentTrackingBrush(this);
    }

    public void Setup(HexCell initCell)
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        initCell.brush = this;
        transform.position = initCell.transform.position;
        cell = initCell;
    }

    public void SetColorSO(ColorSO color_)
    {
        colorSO = color_;
        meshRenderer.material.color = colorSO.color;
    }

    public void ClearColorSO()
    {
        colorSO = null;
    }

    public void Record()
    {
        recorderCell = cell;
    }

    public void Read()
    {
        cell.brush = null;

        cell = recorderCell;
        cell.brush = this;
        transform.parent = null;
        transform.position = cell.transform.position;
        recorderCell = null;
    }
}
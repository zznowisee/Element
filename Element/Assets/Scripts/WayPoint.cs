using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WayPoint : MonoBehaviour
{
    MeshRenderer outter;
    [SerializeField] Color defaultCol;
    [SerializeField] Color endOfLineCol;
    [SerializeField] WirePointType wirePointType;

    [HideInInspector] public Wire wire;
    TextMeshPro dirMark;
    ControlPoint controlPoint;

    private void Awake()
    {
        outter = transform.Find("outter").GetComponent<MeshRenderer>();
        dirMark = transform.Find("dirMark").GetComponent<TextMeshPro>();
    }

    public void Init(Wire wire_, WirePointType type_, string name_)
    {
        wirePointType = type_;
        gameObject.name = name_;
        wire = wire_;

        if(type_ == WirePointType.START)
        {
            dirMark.text = "+";
        }
        else if(type_ == WirePointType.END)
        {
            dirMark.text = "-";
        }
    }

    public void SetToEndOfLine(WirePointType type)
    {
        outter.material.color = endOfLineCol;
        wirePointType = type;
        dirMark.gameObject.SetActive(true);
        if(type == WirePointType.START)
        {
            dirMark.text = "+";
        }
        else
        {
            dirMark.text = "-";
        }
    }

    public void SetToMiddle()
    {
        outter.material.color = defaultCol;
        wirePointType = WirePointType.MIDDLE;
        dirMark.gameObject.SetActive(false);
    }

    public WirePointType GetWirePointType()
    {
        return wirePointType;
    }

    public void SetControlPoint(ControlPoint controlPoint_)
    {
        controlPoint = controlPoint_;
        controlPoint.transform.position = transform.position;
    }

    public void ClearControlPoint()
    {
        controlPoint = null;
    }

    public bool IsEmptyForControlPoint()
    {
        return controlPoint == null;
    }
}

public enum WirePointType
{
    START,
    END,
    MIDDLE
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum TrackControlPointType
{
    START,
    END,
    MIDDLE
}
public class TrackControlPoint : MonoBehaviour
{
/*    [HideInInspector] public bool isReady;
    TrackControlPointType type;
    [HideInInspector] public HexCell cell;
    [SerializeField] TextMeshPro orderMarkText;
    Track track;

    void Awake()
    {
        isReady = false;
    }

    public void Setup(Track track_, TrackControlPointType type_, HexCell cell_)
    {
        track = track_;
        type = type_;
        cell = cell_;

        if(type == TrackControlPointType.START)
        {
            orderMarkText.text = "+";
        }
        else if(type == TrackControlPointType.END)
        {
            orderMarkText.text = "-";
        }
    }

    public void EnterNewCell(HexCell newCell)
    {
        transform.position = newCell.transform.position;

        cell = newCell;
        track.EnterCell(newCell, type);
    }

    public bool IsPreviousCell(HexCell cell)
    {
        return track.IsPreviousCell(type, cell);
    }*/
}

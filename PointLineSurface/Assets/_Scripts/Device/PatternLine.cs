using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLine : MonoBehaviour
{
    public enum PatternLineType { START, END }
    public PatternLineType lineType;
    public HexCell cell;
    [SerializeField] PatternLineMesh patternLineMesh;
    [SerializeField] GameObject linePoint;

    public void Setup(HexCell cell_, Vector3 startPosition_, Color color_, int sortingOrder_, PatternLineType lineType_)
    {
        ProcessManager.Instance.SignColoredHexCell(cell_);

        cell = cell_;
        lineType = lineType_;
        transform.parent = cell.patternLineHolder;
        patternLineMesh.Setup(startPosition_, color_);
        patternLineMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder_;
        linePoint.GetComponent<MeshRenderer>().material.color = color_;
        linePoint.GetComponent<MeshRenderer>().sortingOrder = sortingOrder_;
        if (lineType == PatternLineType.START) linePoint.transform.position = startPosition_;
        else linePoint.transform.position = startPosition_;
    }

    public void Painting(Vector3 endPosition)
    {
        if (lineType == PatternLineType.END)
        {
            linePoint.transform.position = endPosition;
        }
        patternLineMesh.UpdateMesh(endPosition);
    }
}

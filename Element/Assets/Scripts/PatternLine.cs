using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLine : MonoBehaviour
{
    public int sortingOrder;
    public HexCell startCell;
    public HexCell endCell;
    [SerializeField] PatternLineMesh patternLineMesh;
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;

    public void Setup(HexCell cell_, Color color, int sortingOrder_)
    {
        startCell = cell_;
        sortingOrder = sortingOrder_;
        transform.SetParent(startCell.patternLineHolder.transform, true);
        patternLineMesh.Setup(startCell.transform.position, color);
        startPoint.transform.position = startCell.transform.position;
        startPoint.GetComponent<MeshRenderer>().material.color = color;
        endPoint.GetComponent<MeshRenderer>().material.color = color;

        startPoint.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        endPoint.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        patternLineMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }

    public void Painting(Vector3 endPosition)
    {
        endPoint.transform.position = endPosition;
        patternLineMesh.UpdateMesh(endPosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLine : MonoBehaviour
{
    public int sortingOrder;
    public HexCell cell;
    [SerializeField] PatternLineMesh patternLineMesh;
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;

    public void Setup(HexCell cell_, Color color, int sortingOrder_)
    {
        cell = cell_;
        sortingOrder = sortingOrder_;
        transform.SetParent(cell.patternLineHolder.transform, true);
        patternLineMesh.Setup(cell.transform.position, color);
        startPoint.transform.position = cell.transform.position;
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

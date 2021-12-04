using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLine : MonoBehaviour
{
    public HexCell cell;
    [SerializeField] PatternLineMesh patternLineMesh;
    [SerializeField] GameObject startPoint;
    [SerializeField] GameObject endPoint;

    public void Setup(HexCell cell_, Color color)
    {
        cell = cell_;
        patternLineMesh.Setup(cell.transform.position, color);
        startPoint.transform.position = cell.transform.position;
        startPoint.GetComponent<MeshRenderer>().material.color = color;
        endPoint.GetComponent<MeshRenderer>().material.color = color;
    }

    public void Painting(Vector3 endPosition)
    {
        endPoint.transform.position = endPosition;
        patternLineMesh.UpdateMesh(endPosition);
    }
}

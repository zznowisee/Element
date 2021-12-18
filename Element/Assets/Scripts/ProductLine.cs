using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductLine : MonoBehaviour
{

    [SerializeField] GameObject startPoint, endPoint;
    [SerializeField] ProductLineMesh productLineMesh;
    public void Setup(Vector3 start, Vector3 end, Color color_)
    {
        productLineMesh.Setup(start, end, color_);
        startPoint.transform.position = start;
        endPoint.transform.position = end;

        startPoint.GetComponent<MeshRenderer>().material.color = color_;
        endPoint.GetComponent<MeshRenderer>().material.color = color_;
    }
}

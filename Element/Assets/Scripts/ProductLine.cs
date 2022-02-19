using UnityEngine;

public class ProductLine : MonoBehaviour
{

    [SerializeField] GameObject startPoint, endPoint;
    [SerializeField] ProductLineMesh productLineMesh;

    public void Setup(Vector3 startPosition, Vector3 endPosition, Color color_)
    {
        productLineMesh.Setup(startPosition, endPosition, color_);
        startPoint.transform.position = startPosition;
        endPoint.transform.position = endPosition;

        startPoint.GetComponent<MeshRenderer>().material.color = color_;
        endPoint.GetComponent<MeshRenderer>().material.color = color_;
    }
}

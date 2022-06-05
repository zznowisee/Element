using UnityEngine;

public class ProductLine : MonoBehaviour
{
    public void Setup(Vector3 start, Vector3 end, int sorting, ColorType colorType)
    {
        Mesh mesh = MeshHelper.CreateLineMesh(start, end);
        GameObject point = transform.Find("point").gameObject;
        point.transform.localPosition = Vector3.zero;
        point.GetComponent<MeshRenderer>().sortingOrder = sorting;
        point.GetComponent<MeshRenderer>().material.color = ColorManager.Instance.GetColor(colorType, true);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material.color = ColorManager.Instance.GetColor(colorType, true);
        GetComponent<MeshRenderer>().sortingOrder = sorting;
    }
}

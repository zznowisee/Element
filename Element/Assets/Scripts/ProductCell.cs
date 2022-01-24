using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductCell : MonoBehaviour
{
    
    public void Setup(Vector3 position, Color color_)
    {
        Mesh mesh = MeshHelper.CreateHexMesh();
        MeshFilter filter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        filter.mesh = mesh;
        meshRenderer.material.color = color_;
        transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCellMesh : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField] Material defaultMat;
    public void Init(Color color_, bool needCollider)
    {
        Mesh mesh = MeshHelper.CreateHexMesh();
        mesh.name = "Hex Map Mesh";
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMat;
        GetComponent<MeshFilter>().mesh = mesh;
        if (needCollider)
        {
            MeshCollider coll = transform.parent.gameObject.AddComponent<MeshCollider>();
            coll.sharedMesh = mesh;
        }
    }
}

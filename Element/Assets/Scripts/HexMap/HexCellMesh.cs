using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexCellMesh : MonoBehaviour
{
    [SerializeField] Material defaultMat;
    [SerializeField] Material colorMat;
    HexCell hexCell;

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    MeshRenderer meshRenderer;

    public void Init()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Hex Map Mesh";

        hexCell = GetComponentInParent<HexCell>();

        Vector3 center = Vector3.zero;
        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        for (Direction d = Direction.NE; d <= Direction.NW; d++)
        {
            AddTriangle(center,
                        center + HexMatrix.conrners[(int)d],
                        center + HexMatrix.conrners[(int)d + 1]);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshRenderer.material = defaultMat;
        MeshCollider coll = hexCell.gameObject.AddComponent<MeshCollider>();
        coll.sharedMesh = mesh;
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int ti = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(ti);
        triangles.Add(ti + 1);
        triangles.Add(ti + 2);

        uvs.Add(Vector2.zero);
        uvs.Add(Vector2.right);
        uvs.Add(Vector2.right);
    }

    public void Coloring(Color col)
    {
        meshRenderer.material = colorMat;
        meshRenderer.material.color = col;
    }

    public void ResetColor()
    {
        meshRenderer.material = defaultMat;
        meshRenderer.material.color = Color.white;
    }
}

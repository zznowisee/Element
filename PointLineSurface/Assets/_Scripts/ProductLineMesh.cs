using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductLineMesh : MonoBehaviour
{
    float width = 2.5f;
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    Vector3[] points;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public void Setup(Vector3 startPosition, Vector3 endPosition, Color color)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh = new Mesh();

        points = new Vector3[2] { startPosition, endPosition };
        Vector3 dir = (points[1] - points[0]).normalized;
        Vector3 v0 = width / 2f * new Vector3(-dir.y, dir.x) + points[0];
        Vector3 v1 = width / 2f * new Vector3(dir.y, -dir.x) + points[0];
        Vector3 v2 = width / 2f * new Vector3(-dir.y, dir.x) + points[1];
        Vector3 v3 = width / 2f * new Vector3(dir.y, -dir.x) + points[1];
        vertices = new Vector3[4] { v0, v1, v2, v3 };
        triangles = new int[6] { 0, 3, 1, 0, 2, 3 };
        uvs = new Vector2[4] { Vector2.zero, Vector2.one, Vector2.zero, Vector2.one };
        meshRenderer.material.color = color;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}

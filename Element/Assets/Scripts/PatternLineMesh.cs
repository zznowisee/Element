using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLineMesh : MonoBehaviour
{
    float width = 1f;
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    Vector3[] points;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public void Setup(Vector3 startPosition, Color color)
    {
        points = new Vector3[2] { startPosition, startPosition };
        vertices = new Vector3[4];
        triangles = new int[6] { 0, 3, 1, 0, 2, 3 };
        uvs = new Vector2[4] { Vector2.zero, Vector2.one, Vector2.zero, Vector2.one };
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh = new Mesh();
        meshRenderer.material.color = color;
    }

    public void UpdateMesh(Vector3 endPosition)
    {
        points[1] = endPosition;
        Vector3 dir = (points[1] - points[0]).normalized;
        vertices[0] = width / 2f * new Vector3(-dir.y, dir.x) + points[0];
        vertices[1] = width / 2f * new Vector3(dir.y, -dir.x) + points[0];
        vertices[2] = width / 2f * new Vector3(-dir.y, dir.x) + points[1];
        vertices[3] = width / 2f * new Vector3(dir.y, -dir.x) + points[1];

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}

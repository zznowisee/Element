using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class PredictionLine : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    float width = 2.5f;
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetPositions(new Vector3[] { new Vector3(0, 7.5f), new Vector3(10, 7.5f), new Vector3(15,15), new Vector3(20,15) });
    }

    public void SetPositions(Vector3[] positions)
    {
        Mesh mesh = GenerateLineMesh(positions);
        meshFilter.mesh = mesh;
    }

    Mesh GenerateLineMesh(Vector3[] positions)
    {
        Mesh mesh = new Mesh();
        if(positions.Length == 2)
        {
            mesh = MeshHelper.CreateLineMesh(positions[0], positions[1]);
        }
        else
        {
            print("Positions.Lenght != 2");
            Vector3[] vertices = new Vector3[positions.Length * 2];
            int[] triangles = new int[(positions.Length - 1) * 6];
            Vector2[] uvs = new Vector2[vertices.Length];
            int trisIndex = 0;
            for (int a = 0, b = 1, c = 2; c < positions.Length; a++, b++, c++)
            {
                Vector3 p0 = positions[a];
                Vector3 p1 = positions[b];
                Vector3 p2 = positions[c];
                Vector3 p0Tangent = (p1 - p0).normalized;
                Vector3 p2Tangent = (p2 - p1).normalized;
                Vector3 p1Tangent = (p0Tangent + p2Tangent).normalized;
                float angle = (180f - Vector3.Angle(p0Tangent, p2Tangent)) / 2f;

                if (a == 0)
                {
                    Vector3 p0CounterN = new Vector3(-p0Tangent.y, p0Tangent.x);
                    Vector3 p0ClockwiseN = -1 * p0CounterN;
                    Vector3 pa = p0 + p0CounterN * width / 2f;
                    Vector3 pb = p0 + p0ClockwiseN * width / 2f;
                    vertices[0] = pa;
                    vertices[1] = pb;
                    uvs[0] = Vector2.up;
                    uvs[1] = Vector2.zero;
                }

                Vector3 p1CounterN = new Vector3(-p1Tangent.y, p1Tangent.x);
                Vector3 p1ClockwiseN = -1 * p1CounterN;
                Vector3 p1a = p1 + p1CounterN * width / 2f / Mathf.Sin(angle * Mathf.Deg2Rad);
                Vector3 p1b = p1 + p1ClockwiseN * width / 2f / Mathf.Sin(angle * Mathf.Deg2Rad);
                vertices[b * 2] = p1a;
                vertices[b * 2 + 1] = p1b;
                uvs[b * 2] = new Vector2((float)b / (positions.Length - 1), 1f);
                uvs[b * 2 + 1] = new Vector2((float)b / (positions.Length - 1), 0f);

                //tris
                triangles[trisIndex] = a * 2;
                triangles[trisIndex + 1] = a * 2 + 3;
                triangles[trisIndex + 2] = a * 2 + 1;

                triangles[trisIndex + 3] = a * 2;
                triangles[trisIndex + 4] = a * 2 + 2;
                triangles[trisIndex + 5] = a * 2 + 3;

                trisIndex += 6;

                if (c == positions.Length - 1)
                {
                    Vector3 p2CounterN = new Vector3(-p2Tangent.y, p2Tangent.x);
                    Vector3 p2ClockwiseN = -1 * p2CounterN;
                    Vector3 p2c = p2 + p2CounterN * width / 2f;
                    Vector3 p2d = p2 + p2ClockwiseN * width / 2f;
                    vertices[c * 2] = p2c;
                    vertices[c * 2 + 1] = p2d;
                    uvs[c * 2] = Vector2.one;
                    uvs[c * 2 + 1] = Vector2.right;
                    triangles[trisIndex] = b * 2;
                    triangles[trisIndex + 1] = b * 2 + 3;
                    triangles[trisIndex + 2] = b * 2 + 1;

                    triangles[trisIndex + 3] = b * 2;
                    triangles[trisIndex + 4] = b * 2 + 2;
                    triangles[trisIndex + 5] = b * 2 + 3;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
        }
        return mesh;
    }
}

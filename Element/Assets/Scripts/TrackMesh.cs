using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMesh : MonoBehaviour
{
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    List<Vector3> points;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    float width;

    [SerializeField] Material defaultMat;

    public void Setup(float width_)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        points = new List<Vector3>() { Vector3.zero };

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        meshFilter.mesh = mesh = new Mesh();
        mesh.name = "Track Mesh";
        meshRenderer.material = defaultMat;

        width = width_;
    }

    public void EnterPreviousCell(TrackControlPointType type)
    {
        switch (type)
        {
            case TrackControlPointType.START:
                points.RemoveAt(0);
                vertices.RemoveRange(0, 2);
                triangles.RemoveRange(0, 6);
                Vector3 p0 = points[0];
                Vector3 pTangent = (points[1] - points[0]).normalized;
                Vector3 counterN = new Vector3(-pTangent.y, pTangent.x);
                Vector3 clockwiseN = counterN * -1;
                Vector3 a = p0 + counterN * width / 2f;
                Vector3 b = p0 + clockwiseN * width / 2f;

                vertices[0] = a;
                vertices[1] = b;
                for (int i = 0; i < triangles.Count; i++)
                {
                    triangles[i] -= 2;
                }
                uvs.RemoveRange(0, 2);
                break;
            case TrackControlPointType.END:
                points.RemoveAt(points.Count - 1);
                vertices.RemoveRange(vertices.Count - 2, 2);
                triangles.RemoveRange(triangles.Count - 6, 6);
                Vector3 p = points[points.Count - 1];
                Vector3 t = (p - points[points.Count - 2]).normalized;
                Vector3 ccn = new Vector3(-t.y, t.x);
                Vector3 cn = ccn * -1;
                Vector3 c = p + ccn * width / 2f;
                Vector3 d = p + cn * width / 2f;
                vertices[vertices.Count - 2] = c;
                vertices[vertices.Count - 1] = d;
                uvs.RemoveRange(uvs.Count - 2, 2);
                break;
        }

        mesh.triangles = triangles.ToArray();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    public void EnterNewCell(Vector3 cellPosition, TrackControlPointType type)
    {
        Vector3 newPoint = cellPosition - transform.position;
        switch (type)
        {
            case TrackControlPointType.START:
                points.Insert(0, newPoint);
                if(points.Count <= 2)
                {
                    Vector3 p0 = points[0];
                    Vector3 p1 = points[1];
                    Vector3 tangent = (p0 - p1).normalized;
                    Vector3 counterN = new Vector3(-tangent.y, tangent.x);
                    Vector3 clockwiseN = counterN * -1;

                    Vector3 a = p0 + counterN * width / 2f;
                    Vector3 b = p0 + clockwiseN * width / 2f;
                    Vector3 c = p1 + counterN * width / 2f;
                    Vector3 d = p1 + clockwiseN * width / 2f;
                    vertices.AddRange(new List<Vector3> { a, b, c, d });
                    triangles.AddRange(new List<int> { 0, 3, 1, 0, 2, 3 });
                    uvs.AddRange(new List<Vector2> { Vector2.one, Vector2.zero, Vector2.one, Vector2.zero });
                }
                else
                {
                    Vector3 p0 = points[0];
                    Vector3 p1 = points[1];
                    Vector3 p2 = points[2];

                    Vector3 p0Tangent = (p1 - p0).normalized;
                    Vector3 p2Tangent = (p2 - p1).normalized;
                    Vector3 p1Tangent = (p2Tangent + p0Tangent).normalized;
                    Vector3 p0CounterN = new Vector3(-p0Tangent.y, p0Tangent.x);
                    Vector3 p0ClockwiseN = p0CounterN * -1;
                    Vector3 p1CounterN = new Vector3(-p1Tangent.y, p1Tangent.x);
                    Vector3 p1ClockwiseN = p1CounterN * -1;
                    float angle = (180f - Vector3.Angle(p0Tangent, p2Tangent)) / 2f;
                    Vector3 a = p0 + p0CounterN * width / 2f;
                    Vector3 b = p0 + p0ClockwiseN * width / 2f;
                    Vector3 c = p1 + p1CounterN * width / Mathf.Sin(angle * Mathf.Deg2Rad) / 2f;
                    Vector3 d = p1 + p1ClockwiseN * width / Mathf.Sin(angle * Mathf.Deg2Rad) / 2f;

                    AddOneLineAtBegin(a, b, c, d);
                }
                break;
            case TrackControlPointType.END:
                points.Add(newPoint);
                if(points.Count <= 2)
                {
                    Vector3 p0 = points[0];
                    Vector3 p1 = points[1];
                    Vector3 tangent = (p1 - p0).normalized;
                    Vector3 counterN = new Vector3(-tangent.y, tangent.x);
                    Vector3 clockwiseN = counterN * -1;

                    Vector3 a = p0 + counterN * width / 2f;
                    Vector3 b = p0 + clockwiseN * width / 2f;
                    Vector3 c = p1 + counterN * width / 2f;
                    Vector3 d = p1 + clockwiseN * width / 2f;
                    vertices.AddRange(new List<Vector3> { a, b, c, d });
                    triangles.AddRange(new List<int> { 0, 3, 1, 0, 2, 3 });
                    uvs.AddRange(new List<Vector2> { Vector2.one, Vector2.zero, Vector2.one, Vector2.zero });
                }
                else
                {
                    Vector3 p0 = points[points.Count - 3];
                    Vector3 p1 = points[points.Count - 2];
                    Vector3 p2 = points[points.Count - 1];

                    Vector3 p0Tangent = (p1 - p0).normalized;
                    Vector3 p2Tangent = (p2 - p1).normalized;
                    Vector3 p1Tangent = (p0Tangent + p2Tangent).normalized;
                    float angle = (180f - Vector3.Angle(p0Tangent, p2Tangent)) / 2f;
                    Vector3 p1CounterN = new Vector3(-p1Tangent.y, p1Tangent.x);
                    Vector3 p1ClockwiseN = p1CounterN * -1;
                    Vector3 p2CounterN = new Vector3(-p2Tangent.y, p2Tangent.x);
                    Vector3 p2ClockwiseN = p2CounterN * -1;
                    Vector3 a = p1 + p1CounterN * width / Mathf.Sin(angle * Mathf.Deg2Rad) / 2f;
                    Vector3 b = p1 + p1ClockwiseN * width / Mathf.Sin(angle * Mathf.Deg2Rad) / 2f;
                    Vector3 c = p2 + p2CounterN * width / 2f;
                    Vector3 d = p2 + p2ClockwiseN * width / 2f;
                    AddOneLineAtEnd(a, b, c, d);
                }
                break;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
    void AddOneLineAtEnd(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int i = vertices.Count - 2;

        vertices[i] = a;
        vertices[i + 1] = b;
        vertices.Add(c);
        vertices.Add(d);

        triangles.Add(i);
        triangles.Add(i + 3);
        triangles.Add(i + 1);
        triangles.Add(i);
        triangles.Add(i + 2);
        triangles.Add(i + 3);

        uvs.Add(Vector2.one);
        uvs.Add(Vector2.zero);
    }
    void AddOneLineAtBegin(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        vertices[0] = c;
        vertices[1] = d;
        vertices.Insert(0, b);
        vertices.Insert(0, a);

        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i] += 2;
        }

        triangles.Insert(0, 3);
        triangles.Insert(0, 2);
        triangles.Insert(0, 0);
        triangles.Insert(0, 1);
        triangles.Insert(0, 3);
        triangles.Insert(0, 0);

        uvs.Insert(0, Vector2.zero);
        uvs.Insert(0, Vector2.one);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireMesh : MonoBehaviour
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

    public void Init(float width_)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        points = new List<Vector3>() { Vector3.zero };

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        meshFilter.mesh = mesh = new Mesh();
        mesh.name = "Line Mesh";
        meshRenderer.material = defaultMat;

        width = width_;
        //InitMesh();
        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        //mesh.uv = uvs.ToArray();
    }

    void InitMesh()
    {
        Vector3 v = Vector3.zero;

        int ti = vertices.Count;
        vertices.Add(v);
        vertices.Add(v);
        vertices.Add(v);
        vertices.Add(v);

        triangles.Add(ti);
        triangles.Add(ti + 1);
        triangles.Add(ti + 2);
        triangles.Add(ti + 2);
        triangles.Add(ti + 1);
        triangles.Add(ti + 3);

        uvs.Add(Vector2.zero);
        uvs.Add(Vector2.up);
        uvs.Add(Vector2.right);
        uvs.Add(Vector2.one);
    }

    public void UpdateMesh(Vector3 position)
    {
        points[points.Count - 1] = position - transform.position;
        Vector3 dir = (points[points.Count - 1] - points[points.Count - 2]).normalized;
        Vector3 counterClockwiseNormal = new Vector3(-dir.y, dir.x);
        Vector3 clockwiseNormal = new Vector3(dir.y, -dir.x);

        Vector3 a = points[points.Count - 2] + clockwiseNormal * width / 2;
        Vector3 b = points[points.Count - 2] + counterClockwiseNormal * width / 2;
        Vector3 c = points[points.Count - 1] + clockwiseNormal * width / 2;
        Vector3 d = points[points.Count - 1] + counterClockwiseNormal * width / 2;

        vertices[vertices.Count - 4] = a;
        vertices[vertices.Count - 3] = b;
        vertices[vertices.Count - 2] = c;
        vertices[vertices.Count - 1] = d;

        mesh.vertices = vertices.ToArray();
        mesh.RecalculateNormals();
    }

    public void OnEnterNewCell(Vector3 position, WirePointType type)
    {
        if(type == WirePointType.END)
        {
            points.Add(position);
            points[points.Count - 1] = position - transform.position;
            Vector3 dir = (points[points.Count - 1] - points[points.Count - 2]).normalized;
            Vector3 counterClockwiseNormal = new Vector3(-dir.y, dir.x);
            Vector3 clockwiseNormal = new Vector3(dir.y, -dir.x);
            Vector3 a = points[points.Count - 2] + clockwiseNormal * width / 2;
            Vector3 b = points[points.Count - 2] + counterClockwiseNormal * width / 2;
            Vector3 c = points[points.Count - 1] + clockwiseNormal * width / 2;
            Vector3 d = points[points.Count - 1] + counterClockwiseNormal * width / 2;
            int ti = vertices.Count;
            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);
            triangles.Add(ti);
            triangles.Add(ti + 1);
            triangles.Add(ti + 2);
            triangles.Add(ti + 2);
            triangles.Add(ti + 1);
            triangles.Add(ti + 3);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.up);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.up);
        }
        else if(type == WirePointType.START)
        {
            points.Insert(0, position);
            points[0] = position - transform.position;
            Vector3 dir = (points[0] - points[1]).normalized;
            Vector3 counterClockwiseNormal = new Vector3(-dir.y, dir.x);
            Vector3 clockwiseNormal = new Vector3(dir.y, -dir.x);
            Vector3 a = points[0] + clockwiseNormal * width / 2;
            Vector3 b = points[0] + counterClockwiseNormal * width / 2;
            Vector3 c = points[1] + clockwiseNormal * width / 2;
            Vector3 d = points[1] + counterClockwiseNormal * width / 2;
            vertices.Insert(0, c);
            vertices.Insert(0, d);
            vertices.Insert(0, a);
            vertices.Insert(0, b);
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i] += 4;
            }
            triangles.Insert(0, 3);
            triangles.Insert(0, 1);
            triangles.Insert(0, 2);
            triangles.Insert(0, 2);
            triangles.Insert(0, 1);
            triangles.Insert(0, 0);
            uvs.Insert(0, Vector2.zero);
            uvs.Insert(0, Vector2.up);
            uvs.Insert(0, Vector2.zero);
            uvs.Insert(0, Vector2.up);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    public void OnEnterPreviousCell(WirePointType point)
    {
        if(point == WirePointType.START)
        {
            triangles.RemoveAt(0);
            triangles.RemoveAt(0);
            triangles.RemoveAt(0);
            triangles.RemoveAt(0);
            triangles.RemoveAt(0);
            triangles.RemoveAt(0);
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i] -= 4;
            }
            vertices.RemoveAt(0);
            vertices.RemoveAt(0);
            vertices.RemoveAt(0);
            vertices.RemoveAt(0);
            uvs.RemoveAt(0);
            uvs.RemoveAt(0);
            uvs.RemoveAt(0);
            uvs.RemoveAt(0);
            points.RemoveAt(0);
        }
        else if(point == WirePointType.END)
        {
            triangles.RemoveAt(triangles.Count - 1);
            triangles.RemoveAt(triangles.Count - 1);
            triangles.RemoveAt(triangles.Count - 1);
            triangles.RemoveAt(triangles.Count - 1);
            triangles.RemoveAt(triangles.Count - 1);
            triangles.RemoveAt(triangles.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);
            uvs.RemoveAt(uvs.Count - 1);
            uvs.RemoveAt(uvs.Count - 1);
            uvs.RemoveAt(uvs.Count - 1);
            uvs.RemoveAt(uvs.Count - 1);
            points.RemoveAt(points.Count - 1);
        }

        mesh.triangles = triangles.ToArray();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
}

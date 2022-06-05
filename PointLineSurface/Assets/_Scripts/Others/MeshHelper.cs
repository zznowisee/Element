using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshHelper
{
    public static Mesh CreateHexMesh()
    {
        Mesh mesh = new Mesh();

        Vector3 center = Vector3.zero;
        Vector3[] vertices = new Vector3[7];
        Vector2[] uvs = new Vector2[7];
        vertices[0] = center;
        uvs[0] = Vector2.zero;
        List<int> tris = new List<int>();

        int ti = 0;
        void AddTriangle()
        {
            ti++;

            tris.Add(0);
            tris.Add(ti);
            tris.Add(ti + 1 == 7 ? 1 : ti + 1);
        }

        for (Direction d = Direction.NE; d <= Direction.NW; d++)
        {
            vertices[(int)d + 1] = center + HexMatrix.conrners[(int)d];
            uvs[(int)d + 1] = Vector2.right;
            AddTriangle();
        }

        mesh.vertices = vertices;
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh CreateLineMesh(Vector3 start, Vector3 end)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] tris = new int[6];
        Vector3 dir = (end - start).normalized;
        float width = 2.5f;
        Vector3 v0 = width * 0.5f * new Vector3(-dir.y, dir.x) + start;
        Vector3 v1 = width * 0.5f * new Vector3(dir.y, -dir.x) + start;
        Vector3 v2 = width * 0.5f * new Vector3(-dir.y, dir.x) + end;
        Vector3 v3 = width * 0.5f * new Vector3(dir.y, -dir.x) + end;
        vertices = new Vector3[4] { v0, v1, v2, v3 };
        tris = new int[6] { 0, 3, 1, 0, 2, 3 };
        uvs = new Vector2[4] { Vector2.zero, Vector2.one, Vector2.zero, Vector2.one };
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}

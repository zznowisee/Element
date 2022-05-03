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
}

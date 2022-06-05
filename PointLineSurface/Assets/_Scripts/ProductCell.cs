using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductCell : MonoBehaviour
{
    [SerializeField] ProductLine pfProductLine;
    MeshFilter pointFilter;
    MeshRenderer pointRenderer;

    public void Setup(ProductCellInfo info)
    {
        int sortingLayerOrder = 0;
        if (info.template_HasPoint)
        {
            pointFilter = transform.Find("PointMesh").GetComponent<MeshFilter>();
            pointRenderer = transform.Find("PointMesh").GetComponent<MeshRenderer>();
            Mesh cellMesh = MeshHelper.CreateHexMesh();
            pointFilter.mesh = cellMesh;
            pointRenderer.material.color = ColorManager.Instance.GetColor(info.template_PointColorType, true);
            pointRenderer.sortingOrder = sortingLayerOrder++;
        }
        if (info.template_HasLine)
        {
            for (int i = 0; i < info.templateLines.Count; i++)
            {
                ProductLine productLine = Instantiate(pfProductLine, transform);
                Product_Line pl = info.templateLines[i];
                Vector3 start = Vector3.zero;
                Vector3 end = Vector3.zero + (info.cell.GetNeighbor(pl.direction).transform.position - info.cell.transform.position).normalized * HexMatrix.innerRadius;
                productLine.Setup(start, end, sortingLayerOrder++, pl.colorType);
            }
        }

        transform.position = info.cell.transform.position;
    }

    Mesh CombineMeshes(Mesh[] meshes)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < meshes.Length; i++)
        {
            vertices.AddRange(meshes[i].vertices);
            uvs.AddRange(meshes[i].uv);

            int offset = 4 * i;
            foreach (int tri in meshes[i].triangles)
            {
                tris.Add(tri + offset);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}

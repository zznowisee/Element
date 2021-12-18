using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductCell : MonoBehaviour
{
    [SerializeField] HexCellMesh cellMesh;
    public void Setup(Vector3 position, Color color_)
    {
        transform.position = position;
        cellMesh.Init(color_, false);
    }
}

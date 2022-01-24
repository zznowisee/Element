using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexCell : MonoBehaviour
{
    HexCellMesh hexMesh;
    public HexCoordinates hexCoordinates;
    public HexCell[] neighbors;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material defaultMat;
    [SerializeField] Material highlightMat;
    [SerializeField] Color defaultCol;
    public bool beColoring = false;
    public int index;
    public GameObject currentObject;
    public Transform patternLineHolder;
    public void Setup(int index, Vector3 position, Transform parent, HexCoordinates coordinates)
    {
        neighbors = new HexCell[6];
        hexMesh = GetComponentInChildren<HexCellMesh>();
        hexMesh.Init(Color.white, true);
        this.index = index;
        transform.position = position;
        transform.parent = parent;
        hexCoordinates = coordinates;

        gameObject.name = hexCoordinates.ToString();
        bool debug = false;
        if (debug)
        {
            indexText.text = $"{hexCoordinates.X}\n{hexCoordinates.Y}\n{hexCoordinates.Z}";
        }
        else
        {
            indexText.text = "";
        }
    }

    public IReciever GetICommandReciever()
    {
        if(currentObject != null)
        {
            return currentObject.GetComponent<IReciever>();
        }
        return null;
    }

    public Direction GetHexDirection(HexCell cell)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] == cell)
            {
                return (Direction)i;
            }
        }

        return Direction.E;
    }

    public HexCell GetNeighbor(Direction direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(Direction direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexCell PreviousCell(HexCell cell)
    {
        Direction direction = GetHexDirection(cell);
        return neighbors[(int)direction.Previous()];
    }

    public HexCell NextCell(HexCell cell)
    {
        Direction direction = GetHexDirection(cell);
        return neighbors[(int)direction.Next()];
    }

    public bool IsEmpty() => currentObject == null;

    public void PaintingWithColor(Color col, int sortingOrder)
    {
        beColoring = true;
        Coloring(col);
        hexMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        ProcessSystem.Instance.recordCells.Add(this);
    }
    void Coloring(Color col)
    {
        meshRenderer.material = highlightMat;
        meshRenderer.material.color = col;
    }

    void ResetColor()
    {
        meshRenderer.material = defaultMat;
        meshRenderer.material.color = defaultCol;
    }
    public void ResetCell()
    {
        ResetColor();
        for (int i = 0; i < patternLineHolder.childCount; i++)
        {
            Destroy(patternLineHolder.GetChild(i).gameObject);
        }
        beColoring = false;
    }
}

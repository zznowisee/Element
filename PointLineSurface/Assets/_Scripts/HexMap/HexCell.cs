using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexCell : MonoBehaviour
{
    public enum CellState { Normal, Dirty }
    public CellState State { get; set; } = CellState.Normal;
    HexCellMesh hexMesh;
    public HexCoordinates hexCoordinates;
    public HexCell[] neighbors;
    [SerializeField] TextMeshPro indexText;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material defaultMat;
    [SerializeField] Material highlightMat;
    [SerializeField] Color defaultCol;
    public GameObject currentObject;
    public Transform patternLineHolder;

    public void Setup(HexCoordinates coordinates)
    {
        neighbors = new HexCell[6];
        State = CellState.Normal;
        hexCoordinates = coordinates;
        hexMesh = GetComponentInChildren<HexCellMesh>();
        hexMesh.Init(Color.white, true);
        bool debug = false;
        if (debug)
        {
            indexText.text = $"{hexCoordinates.X}\n{hexCoordinates.Y}";
        }
        else
        {
            indexText.text = "";
        }
    }

    public IReciever GetICommandReciever()
    {
        if(currentObject != null && currentObject.transform.TryGetComponent(out IReciever reciever))
        {
            return reciever;
        }
        return null;
    }

    public Device_Extender GetExtender()
    {
        if (currentObject != null && currentObject.transform.TryGetComponent(out Device_Extender extender))
        {
            return extender;
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
        //cell.neighbors[(int)direction.Opposite()] = this;
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
        State = CellState.Dirty;
        Coloring(col);
        hexMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        ProcessManager.Instance.SignColoredHexCell(this);
    }
    void Coloring(Color col)
    {
        meshRenderer.material = highlightMat;
        meshRenderer.material.color = col;
    }

    public void EraseColor()
    {
        //if (State == CellState.Normal) return;
        //clear coloring
        //State = CellState.Normal;
        meshRenderer.material = defaultMat;
        meshRenderer.material.color = defaultCol;
        //clear line
        for (int i = 0; i < patternLineHolder.childCount; i++)
        {
            Destroy(patternLineHolder.GetChild(i).gameObject);
        }

        print("Erase Color - Hex Cell.");
        CheckManager.Instance.CellEraseColor(this);
    }

    public List<ConnectableDevice> GetConnectableDeviceListInNeighbor()
    {
        List<ConnectableDevice> list = new List<ConnectableDevice>();
        for (int i = 0; i < neighbors.Length; i++)
        {
            if(neighbors[i] != null && !neighbors[i].IsEmpty())
            {
                var connectableDevice = neighbors[i].currentObject.GetComponent<ConnectableDevice>();
                if(connectableDevice != null)
                {
                    list.Add(connectableDevice);
                }
            }
        }
        return list;
    }
}

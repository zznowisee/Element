using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexMap : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    [Range(0f,1f)] public float cellScaler = .9f;
    public HexCell pfCell;

    HexCell[] cells;
    Canvas gridHolder;

    void Awake()
    {
        Init();
    }

    void OnValidate()
    {
        if(width <= 0)
        {
            width = 1;
        }    
        if(height <= 0)
        {
            height = 1;
        }
    }

    public void Init()
    {
        cells = new HexCell[width * height];
        gridHolder = GetComponentInChildren<Canvas>();

        string mapHolderName = "MapHolder";
        string textHolderName = "TextHolder";
        if (gridHolder.transform.Find(mapHolderName))
        {
            DestroyImmediate(gridHolder.transform.Find(mapHolderName).gameObject);
        }
        if (gridHolder.transform.Find(textHolderName))
        {
            DestroyImmediate(gridHolder.transform.Find(textHolderName).gameObject);
        }
        Transform mapHolder = new GameObject(mapHolderName).transform;
        Transform textHolder = new GameObject(textHolderName).transform;
        mapHolder.parent = gridHolder.transform;
        textHolder.parent = gridHolder.transform;

        Vector3 originPosition = new Vector3(-width / 2 * HexMatrix.innerRadius * 2f, -height / 2 * HexMatrix.outerRadius * 1.5f);
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(originPosition, x, y, i++, mapHolder, textHolder);
            }
        }
    }

    void CreateCell(Vector3 originPosition, int x, int y, int i, Transform parent, Transform textHolder)
    {
        Vector3 position = Vector3.zero;
        position.x = (x + y * 0.5f - y / 2) * HexMatrix.innerRadius * 2f;
        position.y = y * HexMatrix.outerRadius * 1.5f;
        position += originPosition;

        HexCell cell = cells[i] = Instantiate(pfCell);
        cell.transform.localScale *= cellScaler;
        cell.Setup(position, parent, HexCoordinates.SetCoordinates(x, y));

        if(x > 0)
        {
            cell.SetNeighbor(Direction.W, cells[i - 1]);
        }
        if(y > 0)
        {
            if((y & 1) == 0)
            {
                cell.SetNeighbor(Direction.SE, cells[i - width]);
                if(x > 0)
                {
                    cell.SetNeighbor(Direction.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(Direction.SW, cells[i - width]);
                if(x < width - 1)
                {
                    cell.SetNeighbor(Direction.SE, cells[i - width + 1]);
                }
            }
        }
    }
}

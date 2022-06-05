using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public static MapGenerator Instance { get; private set; }
    public int size = 6;
    public float cellRadius = 5;
    public HexCell pfHexCell;
    private Dictionary<HexCoordinates, HexCell> hexDictionary = new Dictionary<HexCoordinates, HexCell>();
    private Transform cellHolder;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        hexDictionary = new Dictionary<HexCoordinates, HexCell>();

        string mapHolderName = "mapHolder";
        if (transform.Find(mapHolderName))
        {
            DestroyImmediate(transform.Find(mapHolderName).gameObject);
        }
        Transform mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.SetParent(transform, true);

        DrawHexGridLoop(size, mapHolder);
        SetCellsNeighbors();
        cellHolder = mapHolder;
        mapHolder.gameObject.SetActive(false);
    }

    private void DrawHexGridLoop(int n, Transform parent)
    {
        float xoff = Mathf.Cos(Mathf.Deg2Rad * 30f) * cellRadius;
        float yoff = Mathf.Sin(Mathf.Deg2Rad * 30f) * cellRadius;

        int h = n / 2;
        int index = -1;
        for (int row = 0; row < n; row++)
        {
            int cols = n - Mathf.Abs(row - h);

            for (int col = 0; col < cols; col++)
            {
                index++;
                int xIndex = row < h ? col - row : col - h;
                int yIndex = row - h;
                float xPos = (xoff * (col * 2 + 1 - cols));
                float yPos = (yoff * (row - h) * 3);
                CreateCell(xIndex, yIndex, new Vector2(xPos, yPos), parent);
            }
        }
    }
    
    private void CreateCell(int xIndex, int yIndex, Vector2 position, Transform parent)
    {
        HexCell cell = Instantiate(pfHexCell, position, Quaternion.identity, parent);
        HexCoordinates coord = HexCoordinates.SetCoordinates(xIndex, yIndex);
        cell.gameObject.name = coord.ToString();
        cell.Setup(coord);
        hexDictionary[coord] = cell;
    }

    private void SetCellsNeighbors()
    {
        foreach (var cell in hexDictionary.Values)
        {
            for (Direction d = Direction.NE; d <= Direction.NW; d++)
            {
                HexCoordinates neighborCoord = d.HexCoordinateValue() + cell.hexCoordinates;
                cell.SetNeighbor(d, GetHexCell(neighborCoord));
            }
        }
    }

    public HexCell GetHexCell(Vector2Int index)
    {
        HexCoordinates coord = new HexCoordinates(index.x, index.y);
        return GetHexCell(coord);
    }

    public HexCell GetHexCell(int x, int y)
    {
        HexCoordinates coord = new HexCoordinates(x, y);
        return GetHexCell(coord);
    }

    private HexCell GetHexCell(HexCoordinates coord)
    {
        if (!hexDictionary.ContainsKey(coord))
            return null;
        return hexDictionary[coord];
    }

    public void EnableMap()
    {
        cellHolder.gameObject.SetActive(true);
    }

    public void DisableMap()
    {
        cellHolder.gameObject.SetActive(false);
    }
}

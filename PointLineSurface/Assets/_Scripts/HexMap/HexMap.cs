using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{

    public static HexMap Instance { get; private set; }

    public int width = 6;
    public int height = 6;

    [Range(0f,1f)] public float cellScaler = .9f;
    public HexCell pfCell;

    HexCell[] cells;
    private Dictionary<Vector2Int, HexCell> coordCellDictionary;
    public Vector2Int centerCellCoord = new Vector2Int(11, 14);
    public Transform productHolder;

    [SerializeField] UI_OperateScene operatorUISystem;
    public HexCell this[int index] => cells[index];
    public HexCell this[Vector2Int index] => coordCellDictionary[index];
    void Awake()
    {
        Instance = this;
        Init();
    }

    void Start()
    {
        operatorUISystem.OnExitOperateScene += OnSwitchToMainScene;
    }

    private void OnSwitchToMainScene()
    {
        for (int i = 0; i < productHolder.childCount; i++)
        {
            Destroy(productHolder.GetChild(i).gameObject);
        }
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
        coordCellDictionary = new Dictionary<Vector2Int, HexCell>();
        cells = new HexCell[width * height];
        transform.position = new Vector3(300, 0);

        string mapHolderName = "MapHolder";
        if (transform.Find(mapHolderName))
        {
            DestroyImmediate(transform.Find(mapHolderName).gameObject);
        }
        Transform mapHolder = new GameObject(mapHolderName).transform;
        mapHolder.parent = transform;

        Vector3 originPosition = new Vector3(-width / 2 * HexMatrix.innerRadius * 2f, -height / 2 * HexMatrix.outerRadius * 1.5f) + new Vector3(300, 0);
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(originPosition, x, y, i++, mapHolder);
            }
        }
    }

    void CreateCell(Vector3 originPosition, int x, int y, int i, Transform parent)
    {
        Vector3 position = Vector3.zero;
        position.x = (x + y * 0.5f - y / 2) * HexMatrix.innerRadius * 2f;
        position.y = y * HexMatrix.outerRadius * 1.5f;
        position += originPosition;

        HexCell cell = cells[i] = Instantiate(pfCell);
        cell.transform.localScale *= cellScaler;
        HexCoordinates coord = HexCoordinates.SetCoordinates(x, y);
        //cell.Setup(i, position, parent, coord);
        coordCellDictionary[new Vector2Int(coord.X, coord.Y)] = cell;
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

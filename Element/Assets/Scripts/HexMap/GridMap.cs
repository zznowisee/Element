using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public int cycleNum = 3;
    [Range(0f, 1f)] public float cellScaler = .9f;
    public HexCell pfCell;

    HexCell[] cells;
    [SerializeField] Transform cellParent;
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        cells = new HexCell[37];
        Vector3 originPosition = Vector3.zero;
        for (int y = -cycleNum, i = 0; y <= cycleNum; y++)
        {
            for (int x = -cycleNum; x <= cycleNum; x++)
            {
                if(Mathf.Abs(-x - y) > cycleNum)
                {
                    continue;
                }
                CreateCell(originPosition, x, y, i++, cellParent);
            }
        }
    }

    void CreateCell(Vector3 originPosition, int x, int y, int i, Transform parent)
    {
        Vector3 position = Vector3.zero;
        position.x = (x + y * 0.5f) * HexMatrix.innerRadius * 2f;
        position.y = y * HexMatrix.outerRadius * 1.5f;
        position += originPosition;

        HexCell cell = Instantiate(pfCell);
        cell.order = i;
        cell.transform.localScale *= cellScaler;
        cell.Setup(position, parent, HexCoordinates.SetCoordinates(x, y));

    }
}

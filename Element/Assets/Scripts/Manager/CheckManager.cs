using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    public static CheckManager Instance { get; private set; }

    [SerializeField] HexMap map;
    [SerializeField] OperatingRoomUI operatingUI;
    [SerializeField] MainUI mainUI;
    [SerializeField] ProductLine pfProductLine;
    [SerializeField] ProductCell pfProductCell;

    List<ProductColorCell> colorCellList;
    List<ProductLineCell> lineCellList;

    void Awake()
    {
        Instance = this;

        lineCellList = new List<ProductLineCell>();
        colorCellList = new List<ProductColorCell>();
    }

    void Start()
    {
        mainUI.OnSwitchToOperatingRoom += SwitchToOperatingRoom;
        operatingUI.OnSwitchToMainScene += SwitchToMainScene;
    }

    void SwitchToMainScene()
    {
        lineCellList.Clear();
        colorCellList.Clear();
    }

    void SwitchToOperatingRoom(LevelBuildDataSO levelBuildData, LevelData levelData, SolutionData solutionData)
    {
        for (int i = 0; i < levelBuildData.productData.cells.Length; i++)
        {
            var data = levelBuildData.productData.cells[i];
            var cell = map.coordCellDictionary[data.coord + map.centerCellCoord];
            var productCell = Instantiate(pfProductCell, map.productHolder);
            productCell.Setup(cell.transform.position, data.color.initColor);
            var product = new ProductColorCell(cell, data.color.colorType);
            colorCellList.Add(product);
        }
        for (int i = 0; i < levelBuildData.productData.lines.Length; i++)
        {
            var data = levelBuildData.productData.lines[i];
            var cell = map.coordCellDictionary[data.coord + map.centerCellCoord];
            var productLine = Instantiate(pfProductLine, map.productHolder);
            Vector3 endPosition = cell.transform.position + (cell.GetNeighbor(data.direction).transform.position - cell.transform.position).normalized * HexMatrix.innerRadius;
            productLine.Setup(cell.transform.position, endPosition, data.color.initColor);
            var product = new ProductLineCell(cell, data.direction, data.color.colorType);
            lineCellList.Add(product);
        }
    }

    void LineBrushDraw(HexCell start, HexCell end, ColorType colorType)
    {
        for (int i = 0; i < lineCellList.Count; i++)
        {
            if(lineCellList[i].Finish(start, start.GetHexDirection(end), colorType))
            {
                lineCellList[i].finish = true;
            }
        }

        for (int j = 0; j < colorCellList.Count; j++)
        {
            if (colorCellList[j].cell == start || colorCellList[j].cell == end)
            {
                colorCellList[j].finish = false;
                break;
            }
        }
    }

    void ColorBrushDraw(HexCell cell_, ColorType type_)
    {
        for (int i = 0; i < colorCellList.Count; i++)
        {
            var colorProduct = colorCellList[i];
            if (colorProduct.Finish(cell_, type_))
            {
                colorProduct.finish = true;
            }
        }

        for (int i = 0; i < lineCellList.Count; i++)
        {
            var lineProduct = lineCellList[i];
            if (lineProduct.origin == cell_)
            {
                lineProduct.finish = false;
            }
        }
    }

    public bool Complete()
    {
        for (int i = 0; i < colorCellList.Count; i++)
        {
            if (!colorCellList[i].finish)
                return false;
        }

        for (int i = 0; i < lineCellList.Count; i++)
        {
            if (!lineCellList[i].finish)
                return false;
        }

        return true;
    }
}

public class ProductColorCell
{
    public HexCell cell;
    public ColorType colorType;
    public bool finish;
    public ProductColorCell(HexCell cell_, ColorType colorType_)
    {
        cell = cell_;
        colorType = colorType_;
        finish = false;
    }

    public bool Finish(HexCell cell_, ColorType colorType_)
    {
        return cell == cell_ && colorType == colorType_;
    }

    public static bool operator ==(ProductColorCell a, ProductColorCell b)
    {
        return a.cell == b.cell && a.colorType == b.colorType;
    }
    public static bool operator !=(ProductColorCell a, ProductColorCell b)
    {
        return !(a == b);
    }
}

public class ProductLineCell
{
    public HexCell origin;
    public Direction direction;
    public ColorType colorType;
    public bool finish;
    public ProductLineCell(HexCell cell_, Direction direction_, ColorType colorType_)
    {
        origin = cell_;
        direction = direction_;
        colorType = colorType_;
        finish = false;
    }

    public bool Finish(HexCell cell_, Direction direction_, ColorType colorType_)
    {
        if(cell_ == origin && direction == direction_ && colorType == colorType_)
        {
            return finish = true;
        }
        return false;
    }

    public static bool operator == (ProductLineCell a, ProductLineCell b)
    {
        return (a.origin == b.origin) && (a.direction == b.direction) && (a.colorType == b.colorType);
    }
    public static bool operator !=(ProductLineCell a, ProductLineCell b)
    {
        return !(a == b);
    }

}

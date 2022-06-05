using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    public static CheckManager Instance { get; private set; }

    [SerializeField] ProductLine pfProductLine;
    [SerializeField] ProductCell pfProductCell;

    public ProductCellInfo[] productCellTemplates;
    List<ErrorProductCellInfo> errorCellList;

    void Awake()
    {
        Instance = this;
        errorCellList = new List<ErrorProductCellInfo>();
    }

    void Start()
    {
        UI_OperateScene.Instance.OnExitOperateScene += SwitchToMainScene;
    }

    void SwitchToMainScene()
    {

    }

    public void BuildProductCells(Data_Level levelData)
    {
        productCellTemplates = new ProductCellInfo[levelData.productCellDatas.Length];
        for (int i = 0; i < levelData.productCellDatas.Length; i++)
        {
            var data = levelData.productCellDatas[i];
            var cell = MapGenerator.Instance.GetHexCell(data.x, data.y);
            if(cell == null)
            {
                print("Can not find ProductCell's Index");
            }
            var productCell = Instantiate(pfProductCell);
            var cellInfo = new ProductCellInfo(data, cell);
            productCell.Setup(cellInfo);

            productCellTemplates[i] = cellInfo;
        }
    }

    public void LineBrushDraw(HexCell start, HexCell end, ColorType colorType)
    {
        print($"Draw Line From {start} to {end}. - CheckManager");
        Direction startToEnd = start.GetHexDirection(end);
        Direction endToStart = startToEnd.Opposite();

        bool startDrawOnProduct = false;
        bool endDrawOnProduct = false;
        for (int i = 0; i < productCellTemplates.Length; i++)
        {
            if(productCellTemplates[i].cell == start)
            {
                productCellTemplates[i].DrawNewLine(startToEnd, colorType);
                startDrawOnProduct = true;
            }
            else if(productCellTemplates[i].cell == end)
            {
                productCellTemplates[i].DrawNewLine(endToStart, colorType);
                endDrawOnProduct = true;
            }
        }

        if (!startDrawOnProduct)
        {
            bool errorListContains = false;
            for (int i = 0; i < errorCellList.Count; i++)
            {
                if(errorCellList[i].cell == start)
                {
                    errorListContains = true;
                    errorCellList[i].DrawNewLine(startToEnd, colorType);
                    break;
                }
            }
            if (!errorListContains)
            {
                errorCellList.Add(new ErrorProductCellInfo()
                {
                    cell = start,
                    hasLine = true,
                    hasPoint = false,
                    lines = new List<Product_Line>() { new Product_Line() { direction = startToEnd, colorType = colorType } }
                });
            }
        }

        if (!endDrawOnProduct)
        {
            bool errorListContains = false;
            for (int i = 0; i < errorCellList.Count; i++)
            {
                if (errorCellList[i].cell == end)
                {
                    errorListContains = true;
                    errorCellList[i].DrawNewLine(endToStart, colorType);
                    break;
                }
            }
            if (!errorListContains)
            {
                errorCellList.Add(new ErrorProductCellInfo()
                {
                    cell = end,
                    hasLine = true,
                    hasPoint = false,
                    lines = new List<Product_Line>() { new Product_Line() { direction = endToStart, colorType = colorType } }
                });
            }
        }
    }

    public void PointBrushDraw(HexCell cell, ColorType colorType)
    {
        bool drawOnProduct = false;
        for (int i = 0; i < productCellTemplates.Length; i++)
        {
            if(productCellTemplates[i].cell == cell)
            {
                productCellTemplates[i].DrawNewPoint(colorType);
                drawOnProduct = true;
            }
        }
        if (!drawOnProduct)
        {
            bool drawOnErrorCell = false;
            for (int i = 0; i < errorCellList.Count; i++)
            {
                if(errorCellList[i].cell == cell)
                {
                    errorCellList[i].DrawNewPoint(colorType);
                    drawOnErrorCell = true;
                }
            }
            if (!drawOnErrorCell)
            {
                errorCellList.Add(new ErrorProductCellInfo() { cell = cell, hasLine = false, hasPoint = true, pointColorType = colorType });
            }
        }
    }

    public void CellEraseColor(HexCell cell)
    {
        for (int i = 0; i < errorCellList.Count; i++)
        {
            if(errorCellList[i].cell == cell)
            {
                errorCellList.RemoveAt(i);
                return;
            }
        }

        for (int i = 0; i < productCellTemplates.Length; i++)
        {
            if(productCellTemplates[i].cell == cell)
            {
                productCellTemplates[i].ResetComplete();
                break;
            }
        }
    }

    public bool Complete()
    {
        if (errorCellList.Count != 0)
            return false;

        for (int i = 0; i < productCellTemplates.Length; i++)
            if (!productCellTemplates[i].finished)
                return false;

        print("Complete");
        return false;
    }
}

public struct ErrorProductCellInfo
{
    public HexCell cell;
    public bool hasLine;
    public bool hasPoint;
    public ColorType pointColorType;
    public List<Product_Line> lines;

    public void DrawNewLine(Direction direction, ColorType colorType)
    {
        if (lines == null) lines = new List<Product_Line>();

        hasLine = true;
        for (int i = 0; i < lines.Count; i++)
        {
            if(lines[i].direction == direction)
            {
                lines.RemoveAt(i);
                lines.Add(new Product_Line() { direction = direction, colorType = colorType });
                break;
            }
        }
    }

    public void DrawNewPoint(ColorType colorType)
    {
        hasLine = false;
        hasPoint = true;
        if (lines != null) lines.Clear();
        pointColorType = colorType;
    }
}

[System.Serializable]
public struct ProductCellInfo
{
    public HexCell cell;
    public bool template_HasPoint;
    public bool complete_HasPoint;
    public bool template_HasLine;
    public bool complete_HasLine;
    public List<Product_Line> templateLines;
    public List<Product_Line> completeLines;
    public ColorType template_PointColorType;
    public ColorType complete_PointColorType;
    public bool finished;

    public ProductCellInfo(Data_ProductCell data, HexCell cell)
    {
        this.cell = cell;
        template_HasLine = data.hasLine;
        template_HasPoint = data.hasPoint;
        templateLines = new List<Product_Line>();
        completeLines = new List<Product_Line>();
        if (template_HasLine)
        {
            foreach (var line in data.lines)
            {
                templateLines.Add(line);
            }
        }

        template_PointColorType = data.pointColorType;
        complete_PointColorType = data.pointColorType;
        finished = complete_HasLine = complete_HasPoint = false;
    }

    public void ResetComplete()
    {
        completeLines.Clear();
        complete_HasLine = complete_HasPoint = false;
        complete_PointColorType = ColorType.NULL;
        finished = false;
    }

    public void DrawNewLine(Direction direction, ColorType colorType)
    {
        complete_HasLine = true;
        for (int i = 0; i < completeLines.Count; i++)
        {
            if(completeLines[i].direction == direction)
            {
                completeLines.RemoveAt(i);
                completeLines.Add(new Product_Line() { direction = direction, colorType = colorType });
                return;
            }
        }

        completeLines.Add(new Product_Line() { direction = direction, colorType = colorType });
        CheckFinish();
    }

    public void DrawNewPoint(ColorType colorType)
    {
        complete_HasPoint = true;
        complete_HasLine = false;
        complete_PointColorType = colorType;
        completeLines.Clear();
        CheckFinish();
    }

    public void CheckFinish()
    {
        if (finished)
        {
            return;
        }

        if (template_HasPoint != complete_HasPoint || 
            template_HasLine != complete_HasLine || 
            templateLines.Count != completeLines.Count ||
            template_PointColorType != complete_PointColorType)
        {
            finished = false;
        }
        else
        {
            for (int i = 0; i < templateLines.Count; i++)
            {
                if (templateLines[i] != completeLines[i])
                {
                    finished = false;
                    return;
                }
            }
            finished = true;
        }
    }
}
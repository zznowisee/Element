using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BrushBtn : PressDownButton
{
    public Data_BrushBtn data;
    [SerializeField] Image colorImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    private Color brushCol;
    public void Setup(Data_BrushBtn data_, Color brushCol_)
    {
        data = data_;
        brushCol = brushCol_;

        UpdateButtonVisual();

        var tooltipTrigger = GetComponent<TooltipTrigger>();
        switch (data.brushType)
        {
            case BrushType.Point:
                text.text = "Point";
                tooltipTrigger.header = "涂色笔刷";
                tooltipTrigger.content = "状态工具，无法接收指令";
                break;
            case BrushType.Line:
                text.text = "Line";
                tooltipTrigger.header = "画线笔刷";
                tooltipTrigger.content = "状态工具，无法接收指令";
                break;
            case BrushType.Surface:
                text.text = "Surface";
                tooltipTrigger.header = "Surface Brush";
                tooltipTrigger.content = "Surface Brush";
                break;
        }
    }

    void UpdateButtonVisual()
    {
        canvasGroup.alpha = data.number >= 1 ? 1f : 0.01f;
        numberText.text = data.number <= 1 ? "" : $"x{data.number}";
        colorImage.color = brushCol;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
            {
                if (data.number == 0)
                    return;

                BrushCreate();
            }
        }
    }

    void BrushCreate()
    {
        Device_Brush brush = DeviceManager.Instance.NewBrush(data.brushType, data.colorType);
        brush.OnDestory += BrushDestory;
        data.number--;
        UpdateButtonVisual();
    }

    public void BrushDestory(Device_Brush brush)
    {
        data.number++;
        UpdateButtonVisual();
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BrushBtn : PressDownButton
{
    public BrushBtnData data;
    [SerializeField] Image colorImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    public ColorSO colorSO;
    public void Setup(BrushBtnData data_, ColorSO colorSO_)
    {
        data = data_;
        colorSO = colorSO_;

        UpdateButtonVisual();

        var tooltipTrigger = GetComponent<TooltipTrigger>();
        switch (data.brushType)
        {
            case BrushType.Coloring:
                text.text = "涂色笔刷";
                tooltipTrigger.header = "涂色笔刷";
                tooltipTrigger.content = "状态工具，无法接收指令";
                break;
            case BrushType.Line:
                text.text = "画线笔刷";
                tooltipTrigger.header = "画线笔刷";
                tooltipTrigger.content = "状态工具，无法接收指令";
                break;
        }
    }

    void UpdateButtonVisual()
    {
        canvasGroup.alpha = data.number >= 1 ? 1f : 0.01f;
        numberText.text = data.number <= 1 ? "" : $"x{data.number}";
        colorImage.color = colorSO.drawColor;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && MouseManager.Instance.CanClickButton())
            {
                if (data.number == 0)
                    return;

                BrushCreate();
            }
        }
    }

    void BrushCreate()
    {
        Brush brush = DeviceManager.Instance.NewBrush(data.brushType, data.colorType);
        brush.OnDestoryByPlayer += BrushDestory;
        ISelectable selectable = brush.GetComponent<ISelectable>();
        MouseManager.Instance.SetClickedBeforeDragObj(selectable);

        data.number--;
        UpdateButtonVisual();
    }

    public void BrushDestory(Brush brush)
    {
        data.number++;
        UpdateButtonVisual();
    }
}

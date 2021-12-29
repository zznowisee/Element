using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BrushBtn : MonoBehaviour, IPointerDownHandler
{
    public BrushBtnSolutionData brushBtnDataSolution;
    public List<BrushData> brushDatas;
    [SerializeField] Image colorImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    public BrushType brushType;
    public ColorSO colorSO;
    int number;
    public void Setup(BrushBtnSolutionData brushBtnDataSolution_, ColorSO colorSO_)
    {
        brushBtnDataSolution = brushBtnDataSolution_;
        colorSO = colorSO_;
        brushType = brushBtnDataSolution.type;
        number = brushBtnDataSolution.number;
        brushDatas = brushBtnDataSolution.brushDatas;
        TooltipTrigger tooltipTrigger = GetComponent<TooltipTrigger>();
        if (number == 0)
        {
            canvasGroup.alpha = 0.01f;
            canvasGroup.blocksRaycasts = false;
        }
        numberText.text = number <= 1 ? "" : $"x{number}";
        colorImage.color = colorSO.drawColor;
        switch (brushType)
        {
            case BrushType.Coloring:
                text.text = "Ϳɫ��ˢ";
                tooltipTrigger.header = "Ϳɫ��ˢ";
                tooltipTrigger.content = "״̬���ߣ��޷�����ָ��";
                break;
            case BrushType.Line:
                text.text = "���߱�ˢ";
                tooltipTrigger.header = "���߱�ˢ";
                tooltipTrigger.content = "״̬���ߣ��޷�����ָ��";
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            BuildSystem.Instance.CreateNewBrush(this);
            brushBtnDataSolution.number--;
            number--;
            numberText.text = number <= 1 ? "" : $"x{number}";
            if (number == 0)
            {
                canvasGroup.alpha = 0.01f;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void OnDestroyBrush(BrushData brushData_)
    {
        if (number == 0)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        brushBtnDataSolution.number++;
        number++;
        numberText.text = number <= 1 ? "" : $"x{number}";

        brushBtnDataSolution.brushDatas.Remove(brushData_);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BrushBtn : MonoBehaviour, IPointerDownHandler
{
    public BrushBtnData brushBtnData;
    [SerializeField] Image colorImage;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    public BrushType brushType;
    public ColorSO colorSO;
    int number;
    public void Setup(BrushBtnData brushBtnData_)
    {
        brushBtnData = brushBtnData_;
        colorSO = brushBtnData_.colorSO;
        brushType = brushBtnData_.type;
        number = brushBtnData_.number;
        if(number == 0)
        {
            canvasGroup.alpha = 0.01f;
            canvasGroup.blocksRaycasts = false;
        }
        numberText.text = number <= 1 ? "" : $"x{number}";
        colorImage.color = colorSO.color;
        switch (brushType)
        {
            case BrushType.Coloring:
                text.text = "Coloring";
                break;
            case BrushType.Line:
                text.text = "Draw Line";
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BuildSystem.Instance.CreateNewBrush(this);
        brushBtnData.number--;
        number--;
        numberText.text = number <= 1 ? "" : $"x{number}";
        if (number == 0)
        {
            canvasGroup.alpha = 0.01f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void AddBackBrush()
    {
        if(number == 0)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        brushBtnData.number++;
        number++;
        numberText.text = number <= 1 ? "" : $"x{number}";
    }
}

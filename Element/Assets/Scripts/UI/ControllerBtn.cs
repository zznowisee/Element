using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControllerBtn : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    int number;
    public OperatorDataSO data;

    public void Setup(OperatorDataSO data_)
    {
        data = data_;
        number = data_.ControllerNumber;
        numberText.text = number <= 1 ? "" : $"x{number}";
        if (number == 0)
        {
            canvasGroup.alpha = 0.1f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessSystem.Instance.CanOperate())
            {
                BuildSystem.Instance.CreateNewController(this);
                data.ControllerNumber--;
                number--;
                numberText.text = number <= 1 ? "" : $"x{number}";
                if(number == 0)
                {
                    canvasGroup.alpha = 0.1f;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
    }

    public void OnDestroyController()
    {
        if (number == 0)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        data.ControllerNumber++;
        number++;
        numberText.text = number <= 1 ? "" : $"x{number}";
    }
}

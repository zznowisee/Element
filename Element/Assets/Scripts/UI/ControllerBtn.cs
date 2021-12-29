using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControllerBtn : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] CanvasGroup canvasGroup;
    int number;
    public SolutionData data;

    public void Setup(SolutionData data_)
    {
        data = data_;
        number = data_.controllerNumber;
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
                data.controllerNumber--;
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

        data.controllerNumber++;
        number++;
        numberText.text = number <= 1 ? "" : $"x{number}";
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerBtn : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessSystem.Instance.CanOperate())
            {
                BuildSystem.Instance.CreateNewController();
            }
        }
    }
}

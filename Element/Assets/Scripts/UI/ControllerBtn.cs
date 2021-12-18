using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerBtn : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        BuildSystem.Instance.CreateNewController();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class LineBrushBtn : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        BuildSystem.Instance.CreateNewLineBrush();
    }
}

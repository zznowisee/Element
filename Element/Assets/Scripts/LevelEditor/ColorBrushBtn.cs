using UnityEngine;
using UnityEngine.EventSystems;

public class ColorBrushBtn : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        BuildSystem.Instance.CreateNewColorBrush();
    }
}
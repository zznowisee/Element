using UnityEngine;
using UnityEngine.EventSystems;

public class PressDownButton : MonoBehaviour, IPointerDownHandler
{
    public virtual void OnPointerDown(PointerEventData eventData) { }
}

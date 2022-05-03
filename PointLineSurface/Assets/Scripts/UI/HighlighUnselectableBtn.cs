using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HighlighUnselectableBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] Color pointerEnter;
    TextMeshProUGUI text;
    void Awake() => text = transform.Find("text").GetComponent<TextMeshProUGUI>();
    public void OnPointerDown(PointerEventData eventData) => text.color = Color.white;
    public void OnPointerEnter(PointerEventData eventData) => text.color = pointerEnter;
    public void OnPointerExit(PointerEventData eventData) => text.color = Color.white;
}

using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HighlighButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool canBeSelect = false;
    bool selecting = false;
    TextMeshProUGUI text;
    void Awake() => text = transform.Find("text").GetComponent<TextMeshProUGUI>();
    public void OnPointerDown(PointerEventData eventData)
    {
        if (canBeSelect)
        {
            text.color = Color.cyan;
        }
        else
        {
            text.color = Color.white;
        }
    }
    public void OnPointerEnter(PointerEventData eventData) => text.color = Color.cyan;
    public void OnPointerExit(PointerEventData eventData)
    {
        if (canBeSelect)
        {
            if(!selecting)
            {
                selecting = true;
                text.color = Color.white;
            }
        }
        else
        {
            text.color = Color.white;
        }
    }
}

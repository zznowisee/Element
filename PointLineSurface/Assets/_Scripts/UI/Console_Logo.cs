using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Console_Logo : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    private TextMeshProUGUI logoText;
    private Image background;
    private Color normalCol;
    private Color highlightCol;

    private void Awake()
    {
        background = GetComponent<Image>();
        logoText = transform.Find("text").GetComponent<TextMeshProUGUI>();
    }

    public void Setup(string logoText_, Color normalCol_, Color highlightCol_)
    {
        logoText.text = logoText_;
        normalCol = normalCol_;
        highlightCol = highlightCol_;
        background.color = normalCol;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Normal();
    }

    private void Highlight()
    {
        background.color = highlightCol;
    }

    private void Normal()
    {
        background.color = normalCol;
    }
}

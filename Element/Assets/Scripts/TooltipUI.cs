using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] RectTransform backgroundRectTransform;
    [SerializeField] TextMeshProUGUI text;

    public void SetText(string tooltipText)
    {
        text.SetText(tooltipText);
        text.ForceMeshUpdate();
        Vector2 textSize = text.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(8f, 8f);
        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }
}

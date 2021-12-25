using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header;
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] int characterWrapLimit;
    LayoutElement layoutElement;
    RectTransform rectTransform;
    void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content_, string header_ = "")
    {
        UpdatePosition();
        if (string.IsNullOrEmpty(header_))
        {
            header.gameObject.SetActive(false);
        }
        else
        {
            header.gameObject.SetActive(true);
            header.text = header_;
        }

        content.text = content_;
        layoutElement.enabled = header.text.Length > characterWrapLimit || content.text.Length > characterWrapLimit;
    }

    private void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        rectTransform.pivot = new Vector2(0, 1);
        transform.position = position + 10f * Vector2.right;
    }
}

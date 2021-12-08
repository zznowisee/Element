using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    public RectTransform warningBox;
    public TooltipUI warningBoxTooltipUI;

    void Awake()
    {
        Instance = this;
        HideWarning();
    }

    public void ShowWarning(HexCell target, string warningText)
    {
        warningBox.gameObject.SetActive(true);
        Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
        warningBox.position = position;
        warningBoxTooltipUI.SetText(warningText);
    }

    public void HideWarning()
    {
        warningBox.gameObject.SetActive(false);
        warningBoxTooltipUI.SetText("");
    }
}

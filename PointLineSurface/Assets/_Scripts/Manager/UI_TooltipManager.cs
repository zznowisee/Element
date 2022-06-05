using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TooltipManager : MonoBehaviour
{
    public static UI_TooltipManager Instance { get; private set; }
    public UI_Tooltip tooltipUI;
    void Awake()
    {
        Instance = this;
        tooltipUI.gameObject.SetActive(false);
    }

    public static void Show(string content_, string header_ = "")
    {
        Instance.tooltipUI.SetText(content_, header_);
        Instance.tooltipUI.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Instance.tooltipUI.gameObject.SetActive(false);
    }
}

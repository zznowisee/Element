using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    public RectTransform warningBox;
    public WarningUI warningUI;
    public TooltipUI tooltipUI;


    void Awake()
    {
        Instance = this;
        HideWarning();
        tooltipUI.gameObject.SetActive(false);
    }

    public void ShowWarning(Vector3 target, WarningType warningType)
    {
        warningBox.gameObject.SetActive(true);
        Vector3 position = Camera.main.WorldToScreenPoint(target);
        warningBox.position = position;
        string warningText = "";
        switch (warningType)
        {
            case WarningType.BrushConnectedByTwoConnectors:
                warningText = "#Error-00\nThe brush is connected by two connectors at the same time!";
                break;
            case WarningType.Collision:
                warningText = "#Error-01\nDevices collided!";
                break;
            case WarningType.EnteredColoredUnit:
                warningText = "#Error-02\nEntered the unit that has been colored!";
                break;
            case WarningType.ReceiveTwoMoveCommands:
                warningText = "#Error-03\nTwo move commands were received at the same time!";
                break;
            case WarningType.WrongColoring:
                warningText = "#Error-04\nWrong Coloring";
                break;
            case WarningType.WrongLine:
                warningText = "#Error-05\nWrong Line";
                break;
        }
        warningUI.SetText(warningText);
    }

    public void HideWarning()
    {
        warningBox.gameObject.SetActive(false);
        warningUI.SetText("");
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

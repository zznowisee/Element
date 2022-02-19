using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

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
                warningText = "#错误-00\n笔刷同时被两个连接器连接";
                break;
            case WarningType.Collision:
                warningText = "#错误-01\n工具碰撞";
                break;
            case WarningType.EnteredColoredUnit:
                warningText = "#错误-02\n连接器或控制器进入了已经着色过的区域";
                break;
            case WarningType.ReceiveTwoMoveCommands:
                warningText = "#错误-03\n工具同时接收到两个移动指令";
                break;
            case WarningType.WrongColoring:
                warningText = "#错误-04\n在规定外的区域着色";
                break;
            case WarningType.WrongLine:
                warningText = "#错误-05\n在规定外的区域画线";
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

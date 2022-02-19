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
                warningText = "#����-00\n��ˢͬʱ����������������";
                break;
            case WarningType.Collision:
                warningText = "#����-01\n������ײ";
                break;
            case WarningType.EnteredColoredUnit:
                warningText = "#����-02\n��������������������Ѿ���ɫ��������";
                break;
            case WarningType.ReceiveTwoMoveCommands:
                warningText = "#����-03\n����ͬʱ���յ������ƶ�ָ��";
                break;
            case WarningType.WrongColoring:
                warningText = "#����-04\n�ڹ涨���������ɫ";
                break;
            case WarningType.WrongLine:
                warningText = "#����-05\n�ڹ涨���������";
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UI_WarningManager : MonoBehaviour
{
    public static UI_WarningManager Instance { get; private set; }
    public event Action OnWarning;
    [SerializeField] private UI_Warning warningUI;
    [SerializeField] private RectTransform warningBox;
    private void Awake() => Instance = this;
    private void Start() => HideWarning();

    public static void ShowWarning(Vector3 worldPosition, WarningType warningType)
    {
        Instance.warningBox.gameObject.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Instance.warningBox.position = screenPosition;
        string warningText = "";
        switch (warningType)
        {
            case WarningType.Collision:
                warningText = "#Error-01\nDevice Collision";
                break;
            case WarningType.ReceiveTwoMoveCommands:
                warningText = "#Error-02\nDevice Received Two Move Commands At Same Time!";
                break;
            case WarningType.ConnectedByTwo:
                warningText = "#Error-03\nBrush Connected By Two Connectors At Same Time!";
                break;
        }

        Instance.OnWarning?.Invoke();
        Instance.warningUI.SetText(warningText);
    }

    public static void HideWarning()
    {
        Instance.warningBox.gameObject.SetActive(false);
        Instance.warningUI.SetText("");
    }
}

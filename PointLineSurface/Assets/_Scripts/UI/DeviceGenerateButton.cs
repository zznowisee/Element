using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceGenerateButton : PressDownButton
{
    protected TMPro.TextMeshProUGUI numberText;
    protected CanvasGroup canvasGroup;
    protected Data_Solution data;
    protected void Awake()
    {
        numberText = transform.Find("numberText").GetComponent<TMPro.TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(Data_Solution solutionData_)
    {
        data = solutionData_;
        UpdateVisual();
    }

    protected void UpdateVisual()
    {
        numberText.text = data.levelBuildData.controllerLeftNumber <= 1 ? "" : $"x{data.levelBuildData.controllerLeftNumber}";
        if (data.levelBuildData.controllerLeftNumber == 0)
        {
            canvasGroup.alpha = 0.1f;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

    }
}

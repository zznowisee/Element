using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class ProcessBtn : PressDownButton
{
    protected Action callback;
    protected CanvasGroup canvasGroup;
    public void SetButtonCallback(Action callback_) => callback = callback_;
    public override void OnPointerDown(PointerEventData eventData) { }
    public void Enable()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void Disable()
    {
        canvasGroup.alpha = 0.1f;
        canvasGroup.blocksRaycasts = false;
    }
}

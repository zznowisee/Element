using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class StepBtn : ProcessBtn
{
    private Action buttonCallback;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        callback();
    }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class StopBtn : ProcessBtn
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        callback();
        base.OnPointerDown(eventData);
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
}

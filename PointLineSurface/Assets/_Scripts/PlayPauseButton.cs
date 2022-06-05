using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayPauseButton : Button
{

    private TMPro.TextMeshProUGUI text;
    public bool PLAY;

    protected override void Awake()
    {
        base.Awake();
        text = transform.Find("text").GetComponent<TMPro.TextMeshProUGUI>();
        PLAY = true;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (interactable)
        {
            PLAY = !PLAY;
            text.text = PLAY ? "PLAY" : "PAUSE";
        }
    }

    public void SetPlay()
    {
        PLAY = true;
        text.text = "PLAY";
    }

    public void SetPause()
    {
        PLAY = false;
        text.text = "PAUSE";
    }
}

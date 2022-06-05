using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlayPauseBtn : ProcessBtn
{
    private Image icon;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;
    public bool IsPlay;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        icon = transform.Find("icon").GetComponent<Image>();
        IsPlay = true;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        callback();
        if (IsPlay)
        {
            SetPause();
            IsPlay = false;
        }
        else
        {
            SetPlay();
            IsPlay = true;
        }
    }

    public void SetPause()
    {
        icon.sprite = pauseIcon;
        IsPlay = false;
    }

    public void SetPlay()
    {
        icon.sprite = playIcon;
        IsPlay = true;
    }
}

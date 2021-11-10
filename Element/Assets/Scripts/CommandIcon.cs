using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommandIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CommandSO commandSO { get; private set; }
    [SerializeField] Image icon;

    CanvasGroup canvasGroup;
    Vector3 position;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        position = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = InputHelper.MouseWorldPositionIn2D;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = position;
        canvasGroup.blocksRaycasts = true;
    }

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        icon.sprite = commandSO.icon;
    }
}

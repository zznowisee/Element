using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CommandSO commandSO { get; private set; }
    [SerializeField] Image icon;
    CodeSlot codeSlot;
    CanvasGroup canvasGroup;
    Transform draggingParent;
    bool dropValid = false;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        draggingParent = FindObjectOfType<UISystem>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dropValid = false;
        canvasGroup.blocksRaycasts = false;
        transform.parent = draggingParent;
        codeSlot.ClearCommand();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = InputHelper.MouseWorldPositionIn2D;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dropValid)
        {
            Destroy(gameObject);
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        icon.sprite = commandSO.icon;
        dropValid = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void DroppedOnSlot(CodeSlot codeSlot_)
    {
        codeSlot = codeSlot_;
        transform.position = codeSlot.transform.position;
        transform.parent = codeSlot.transform;
        dropValid = true;
    }
}

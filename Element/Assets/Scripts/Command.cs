using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CommandSO commandSO { get; private set; }
    [SerializeField] Image icon;
    CommandSlot commandSlot;
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
        print("Dragging");
        dropValid = false;
        canvasGroup.blocksRaycasts = false;
        transform.parent = draggingParent;
        commandSlot.ClearCommand();
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

    public void DroppedOnSlot(CommandSlot commandSlot_)
    {
        commandSlot = commandSlot_;
        if (!commandSlot.IsEmpty())
        {
            Destroy(commandSlot.GetCommand().gameObject);
            commandSlot.ClearCommand();
        }

        transform.position = commandSlot.transform.position;
        transform.parent = commandSlot.transform;
        canvasGroup.blocksRaycasts = true;
        dropValid = true;

        commandSlot.SetCommand(this);
        commandSlot.commandConsole.RecordCommand(commandSlot.index, this);
    }
}

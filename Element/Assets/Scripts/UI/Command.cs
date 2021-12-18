using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CommandSO commandSO { get; private set; }
    CommandSlot commandSlot;
    CanvasGroup canvasGroup;
    Transform draggingParent;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        draggingParent = FindObjectOfType<OperatorUISystem>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            print("Dragging");
            canvasGroup.blocksRaycasts = false;
            transform.parent = draggingParent;
            commandSlot.ClearCommand();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            transform.position = InputHelper.MouseWorldPositionIn2D;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            CommandSlot slot = InputHelper.GetCommandSlotUnderPosition2D();
            if (slot != null)
            {
                DroppedOnSlot(slot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        GetComponent<Image>().sprite = commandSO.icon;
        canvasGroup.blocksRaycasts = false;
    }

    public void DroppedOnSlot(CommandSlot commandSlot_)
    {
        commandSlot = commandSlot_;
        if (!commandSlot.IsEmpty())
        {
            if(commandSlot.command.commandSO == commandSO)
            {
                Destroy(gameObject);
                return;
            }

            Destroy(commandSlot.command.gameObject);
            commandSlot.ClearCommand();
        }

        transform.position = commandSlot.transform.position;
        transform.parent = commandSlot.transform;
        transform.SetSiblingIndex(0);
        canvasGroup.blocksRaycasts = true;

        commandSlot.SetCommand(this);
    }
}

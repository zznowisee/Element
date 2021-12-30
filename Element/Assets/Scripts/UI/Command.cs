using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour, IPointerDownHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (InputHelper.IsTheseKeysHeld(KeyCode.Delete))
            {
                if (commandSlot != null)
                {
                    commandSlot.ClearCommand();
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                transform.parent = OperatorUISystem.Instance.transform;
                if (commandSlot != null)
                {
                    commandSlot.ClearCommand();
                }
                OperatorUISystem.Instance.SetCurrentTrackingCommand(this);
            }
        }
    }

    public void BeginDrag()
    {
        transform.parent = OperatorUISystem.Instance.transform;
        canvasGroup.blocksRaycasts = false;
        transform.parent = draggingParent;
        if(commandSlot != null)
        {
            commandSlot.ClearCommand();
        }
    }

    public void TryToDropOnSlot()
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
            print("Slot not empty");
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

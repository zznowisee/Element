using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Command : MonoBehaviour, ISelectable
{
    public CommandSO commandSO { get; private set; }
    public SelectableType type { get; set; }
    public bool selected { get; set; }
    public bool preSelected { get; set; }
    public Vector3 delta { get; set; }

    CommandSlot commandSlot;
    CanvasGroup canvasGroup;
    Transform draggingParent;
    GameObject selecting;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        draggingParent = FindObjectOfType<OperatingRoomUI>().transform;
        selecting = transform.Find("selecting").gameObject;
    }

    void Update()
    {
        SelectingVisual();
    }

    void OnEnable() => MouseManager.Instance.AddISelectableObj(this);
    void OnDisable() => MouseManager.Instance.RemoveISelectableObj(this);
    public void SelectingVisual()
    {
        selecting.gameObject.SetActive(selected || preSelected);
    }

    public void TryToDropOnSlot()
    {
        if (ProcessManager.Instance.CanOperate())
        {
            CommandSlot slot = InputHelper.GetCommandSlotUnder(transform.position);
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
        GetComponent<Image>().sprite = commandSO_.icon;
    }

    public void DroppedOnSlot(CommandSlot commandSlot_)
    {
        commandSlot = commandSlot_;
        commandSlot.DroppedOn(this);

        transform.position = commandSlot.transform.position;
        transform.parent = commandSlot.transform;
        transform.SetSiblingIndex(0);
        canvasGroup.blocksRaycasts = true;
    }

    public void LeftClick()
    {
        canvasGroup.blocksRaycasts = false;
        transform.parent = draggingParent;

        if(commandSlot != null)
        {
            commandSlot.ClearCommand();
            commandSlot = null;
        }
    }

    public void RightClick()
    {
        DeleteCommand();
    }

    public void Dragging()
    {
        transform.position = InputHelper.MouseWorldPositionIn2D;
    }

    public void LeftRelease()
    {
        TryToDropOnSlot();
    }

    void DeleteCommand()
    {
        commandSlot.ClearCommand();
        Destroy(gameObject);
    }

    public void DraggingWithList(Vector3 position)
    {
        transform.position = position;
    }
}

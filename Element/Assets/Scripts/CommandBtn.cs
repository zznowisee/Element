using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommandBtn : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public CommandSO commandSO { get; private set; }
    public Command pfCommand;
    [SerializeField] Image icon;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Command command = Instantiate(pfCommand, transform.position, Quaternion.identity, UISystem.Instance.transform);
        command.Setup(commandSO);
        eventData.pointerDrag = command.gameObject;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        icon.sprite = commandSO.icon;
    }
}

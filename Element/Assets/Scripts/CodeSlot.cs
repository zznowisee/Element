using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CodeSlot : MonoBehaviour, IDropHandler
{
    Color normalCol;
    Color highlightCol;
    [SerializeField] Image background;

    Command command;
    CodeConsole console;

    public event Action<CodeSlot, CommandSO> OnCommandIconDropped;
    public event Action<CodeSlot, Command> OnCommandDropped;

    public int index { get; private set; }

    public void Setup(Color normalCol_, Color highlightCol_, int index_, CodeConsole console_)
    {
        normalCol = normalCol_;
        highlightCol = highlightCol_;
        index = index_;
        console = console_;
    }

    public void SetHighlight()
    {
        background.color = highlightCol;
    }

    public void SetNormal()
    {
        background.color = normalCol;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsEmpty())
        {
            CommandIcon draggingCommand = eventData.pointerDrag.GetComponent<CommandIcon>();
            if (draggingCommand != null)
            {
                OnCommandIconDropped?.Invoke(this, draggingCommand.commandSO);
                return;
            }

            Command newCommand = eventData.pointerDrag.GetComponent<Command>();
            if (newCommand != null)
            {
                OnCommandDropped?.Invoke(this, newCommand);
            }
        }
    }

    bool IsEmpty()
    {
        return command == null;
    }

    public Command GetCommand()
    {
        return command;
    }

    public void SetCommand(Command command_)
    {
        command = command_;
    }

    public void ClearCommand()
    {
        command = null;
        console.RecordCommand(index, null);
    }
}

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
    Image background;

    Command command;
    CommandConsole brushConsole;

    public event Action<CodeSlot, Command> OnCommandDropped;

    public int index { get; private set; }

    public void Setup(Color normalCol_, Color highlightCol_, int index_, CommandConsole brushConsole_)
    {
        background = GetComponent<Image>();
        normalCol = normalCol_;
        highlightCol = highlightCol_;
        index = index_;
        brushConsole = brushConsole_;
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
        brushConsole.RecordCommand(index, null);
    }
}

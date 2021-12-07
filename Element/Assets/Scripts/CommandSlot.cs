using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommandSlot : MonoBehaviour
{
    Color normalCol;
    Color highlightCol;
    Image background;

    [HideInInspector] public Command command;
    [HideInInspector] public CommandConsole commandConsole;

    public int index { get; private set; }

    public void Setup(Color normalCol_, Color highlightCol_, int index_, CommandConsole brushConsole_)
    {
        background = GetComponent<Image>();
        normalCol = normalCol_;
        highlightCol = highlightCol_;
        index = index_;
        commandConsole = brushConsole_;
    }

    public void SetHighlight()
    {
        background.color = highlightCol;
    }

    public void SetNormal()
    {
        background.color = normalCol;
    }

    public bool IsEmpty()
    {
        return command == null;
    }

    public void SetCommand(Command command_)
    {
        command = command_;
        commandConsole.RecordCommand(index, command);
    }

    public void ClearCommand()
    {
        command = null;
        commandConsole.RecordCommand(index, null);
    }
}

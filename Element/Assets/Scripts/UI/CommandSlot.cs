using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommandSlot : MonoBehaviour
{
    Image background;
    [SerializeField] Image highlightImage;
    [HideInInspector] public Command command;
    [HideInInspector] public CommandConsole commandConsole;

    public int index { get; private set; }

    public void Setup(Color normalCol_, Color highlightCol_, int index_, CommandConsole brushConsole_)
    {
        background = GetComponent<Image>();
        index = index_;

        background.color = normalCol_;
        highlightImage.color = highlightCol_;

        commandConsole = brushConsole_;
    }

    public void SetHighlight()
    {
        highlightImage.gameObject.SetActive(true);
    }

    public void SetNormal()
    {
        highlightImage.gameObject.SetActive(false);
    }

    public bool IsEmpty()
    {
        return command == null;
    }

    public void SetCommand(Command command_)
    {
        command = command_;
        commandConsole.RecordCommand(index, command.commandSO);
    }

    public void ClearCommand()
    {
        command = null;
        commandConsole.RecordCommand(index, null);
    }
}

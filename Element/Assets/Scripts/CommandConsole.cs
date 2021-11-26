using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandConsole : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI indexText;
    [SerializeField] CommandReader commandReader;
    [HideInInspector] public int index;

    [SerializeField] Color codeSlotNormalCol;
    [SerializeField] Color codeSlotHighlightCol;
    [SerializeField] CodeSlot pfCodeSlot;
    [HideInInspector] public CodeSlot[] slots;
    public Command[] commands;
    public int slotNum = 18;
    public void Setup(CommandReader connecter_)
    {
        commandReader = connecter_;
        slots = new CodeSlot[slotNum];
        commands = new Command[slotNum];
        index = commandReader.index;
        indexText.text = index.ToString();
        gameObject.name = $"Connecter{index}_Console";

        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].OnCommandDropped += ConnecterConsole_OnCommandDropped;
            slots[i].gameObject.name = $"CodeSlot_{ i }";
        }
    }

    public CommandSO GetCommandSOFromLineIndex(int lineIndex)
    {
        if(commands[lineIndex] != null)
        {
            return commands[lineIndex].commandSO;
        }

        return null;
    }

    private void ConnecterConsole_OnCommandDropped(CodeSlot slot, Command command)
    {
        slot.SetCommand(command);
        command.DroppedOnSlot(slot);
        RecordCommand(slot.index, command);
    }

    public void RecordCommand(int index, Command newCommand)
    {
        commands[index] = newCommand;
    }

    public int GetLastCommandIndex()
    {
        int lastCommandIndex = 0;
        for (int i = 0; i < commands.Length; i++)
        {
            if(commands[i] != null)
            {
                if(i >= lastCommandIndex)
                {
                    lastCommandIndex = i;
                }
            }
        }

        return lastCommandIndex;
    }
}

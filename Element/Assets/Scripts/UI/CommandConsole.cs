using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandConsole : MonoBehaviour
{

    public ConsoleData consoleData;

    [SerializeField] TextMeshProUGUI indexText;
    [SerializeField] Controller controller;
    [HideInInspector] public int index;

    [SerializeField] Color codeSlotNormalCol;
    [SerializeField] Color codeSlotHighlightCol;
    [SerializeField] CommandSlot pfCodeSlot;
    [HideInInspector] public CommandSlot[] slots;
    public CommandSO[] commands;
    public int slotNum = 40;

    public void Setup(Controller controller_)
    {
        controller = controller_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        index = controller.index;
        indexText.text = index.ToString();
        gameObject.name = $"Connecter{index}_Console";

        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].gameObject.name = $"CodeSlot_{ i }";
        }
        consoleData.commandSOs = new CommandSO[40];
        consoleData.consoleIndex = index;
        controller.controllerData.consoleData = consoleData;
    }

    public void Setup(Controller controller_, ConsoleData consoleData_)
    {
        controller = controller_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        index = controller.index;
        indexText.text = index.ToString();
        gameObject.name = $"Connecter{index}_Console";

        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].gameObject.name = $"CodeSlot_{ i }";
        }
        consoleData = consoleData_;
    }

    public CommandSO GetCommandSOFromLineIndex(int lineIndex)
    {
        if(commands[lineIndex] != null)
        {
            return commands[lineIndex];
        }

        return null;
    }

    public void RecordCommand(int index, CommandSO newCommand)
    {
        commands[index] = newCommand;
        consoleData.commandSOs[index] = newCommand;
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

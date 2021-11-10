using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeConsole : MonoBehaviour
{
    public int slotNum = 5;

    [SerializeField] TextMeshProUGUI indexText;
    [SerializeField] Color codeSlotNormalCol;
    [SerializeField] Color codeSlotHighlightCol;
    [SerializeField] CodeSlot pfCodeSlot;
    [SerializeField] Command pfCommand;
    CodeSlot[] slots;
    public Command[] commands;
    CodeSlot currentPointer;
    ControlPoint controlPoint;

    public void Setup(ControlPoint controlPoint_)
    {
        slots = new CodeSlot[slotNum];
        commands = new Command[slotNum];
        controlPoint = controlPoint_;
        indexText.text = controlPoint.index.ToString();
        gameObject.name = $"Console_{controlPoint.index}";
        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].OnCommandIconDropped += CodeConsole_OnCommandIconDropped;
            slots[i].OnCommandDropped += CodeConsole_OnCommandDropped;
            slots[i].gameObject.name = $"CodeSlot_{ i }";
        }
    }

    public void RecordCommand(int index, Command command)
    {
        commands[index] = command;
    }

    private void CodeConsole_OnCommandDropped(CodeSlot slot, Command command)
    {
        slot.SetCommand(command);
        command.DroppedOnSlot(slot);

        RecordCommand(slot.index, command);
    }

    private void CodeConsole_OnCommandIconDropped(CodeSlot slot, CommandSO commandSO)
    {
        Command command = Instantiate(pfCommand, slot.transform.position, Quaternion.identity, slot.transform);
        slot.SetCommand(command);
        command.Setup(commandSO, slot);

        RecordCommand(slot.index, command);
    }
}

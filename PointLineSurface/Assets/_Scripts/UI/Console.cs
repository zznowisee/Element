using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{

    public ConsoleData consoleData;
    private Console_Logo consoleLogo;
    private Transform slotHolder;
    [SerializeField] Device_Controller controller;
    [SerializeField] Color codeSlotNormalCol;
    [SerializeField] Color codeSlotHighlightCol;
    [SerializeField] Color logoNormalCol;
    [SerializeField] Color logoHighlightCol;
    [SerializeField] CommandSlot pfCodeSlot;
    [HideInInspector] CommandSlot[] slots;
    CommandSO[] commands;
    public int slotNum = 40;
    public int? highlightIndex;

    private void Awake()
    {
        consoleLogo = GetComponentInChildren<Console_Logo>();
        slotHolder = transform.Find("slotHolder");
    }

    public void Init(Device_Controller controller_)
    {
        controller = controller_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        consoleLogo.Setup(controller.Index.ToString(), logoNormalCol, logoHighlightCol);
        gameObject.name = $"Connector{controller.Index}_Console";

        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, slotHolder);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].gameObject.name = $"CodeSlot_{ i }";
            slots[i].OnCommandChanged += RecordCommand;
        }
        consoleData.commandTypes = new CommandType[slotNum];
        controller.data.consoleData = consoleData;
        transform.SetSiblingIndex(controller.Index);
    }

    public void Rebuild(Device_Controller controller_, ConsoleData consoleData_, Command[] rebuildCommands)
    {
        controller = controller_;
        consoleData = consoleData_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        consoleLogo.Setup(controller.Index.ToString(), logoNormalCol, logoHighlightCol);
        gameObject.name = $"Connecter{controller.Index}_Console";
        print("Rebuild Console");
        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, slotHolder);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].gameObject.name = $"CodeSlot_{ i }";
            slots[i].OnCommandChanged += RecordCommand;
        }

        for (int i = 0; i < rebuildCommands.Length; i++)
        {
            if (rebuildCommands[i] == null)
                continue;

            rebuildCommands[i].DroppedOnSlot(slots[i]);
        }
    }

    public CommandType GetCommand(int lineIndex)
    {
        if(commands[lineIndex] != null)
        {
            return commands[lineIndex].type;
        }
        else
        {
            return CommandType.Delay;
        }
    }

    void RecordCommand(int index, CommandSO newCommand)
    {
        commands[index] = newCommand;
        if (newCommand != null)
        {
            consoleData.commandTypes[index] = newCommand.type;
        }
        else
        {
            consoleData.commandTypes[index] = CommandType.Empty;
        }
    }

    public int GetMaxCommandIndex()
    {
        int maxIndex = -1;
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i])
            {
                if(i > maxIndex)
                {
                    maxIndex = i;
                }
            }
        }
        return maxIndex;
    }

    public void ResetAll()
    {
        if(highlightIndex != null)
        {
            slots[(int)highlightIndex].SetNormal();
            highlightIndex = null;
        }
    }

    public void ExecuteCommand(int lineIndex_)
    {
        if(highlightIndex != null) slots[(int)highlightIndex].SetNormal();
        highlightIndex = lineIndex_;
        slots[lineIndex_].SetHighlight();
    }
}

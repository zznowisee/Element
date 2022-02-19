
using UnityEngine;
using TMPro;

public class CommandConsole : MonoBehaviour
{

    public ConsoleData consoleData;

    [SerializeField] TextMeshProUGUI indexText;
    [SerializeField] Controller controller;
    [SerializeField] Color codeSlotNormalCol;
    [SerializeField] Color codeSlotHighlightCol;
    [SerializeField] CommandSlot pfCodeSlot;
    [HideInInspector] CommandSlot[] slots;
    CommandSO[] commands;
    public int slotNum = 40;
    public int? highlightIndex;
    public int MaxCommandIndex => GetLastCommandIndex();

    public void Setup(Controller controller_)
    {
        controller = controller_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        indexText.text = controller.index.ToString();
        gameObject.name = $"Connector{controller.index}_Console";

        controller.OnDestoryByPlayer += Controller_OnDestory;

        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
            slots[i].Setup(codeSlotNormalCol, codeSlotHighlightCol, i, this);
            slots[i].gameObject.name = $"CodeSlot_{ i }";
            slots[i].OnCommandChanged += RecordCommand;
        }
        consoleData.commandTypes = new CommandType[slotNum];
        controller.data.consoleData = consoleData;
        transform.SetSiblingIndex(controller.index);
    }

    private void Controller_OnDestory(Controller obj)
    {
        Destroy(gameObject);
    }

    public void Rebuild(Controller controller_, ConsoleData consoleData_, Command[] rebuildCommands)
    {
        controller = controller_;
        consoleData = consoleData_;
        slots = new CommandSlot[slotNum];
        commands = new CommandSO[slotNum];
        indexText.text = controller.index.ToString();
        gameObject.name = $"Connecter{controller.index}_Console";

        controller.OnDestoryByPlayer += Controller_OnDestory;
        print("Rebuild Console");
        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = Instantiate(pfCodeSlot, transform);
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

    public CommandType GetCommandSOFromLineIndex(int lineIndex)
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

    int GetLastCommandIndex()
    {
        int lastCommandIndex = -1;
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

    public void ResetAll()
    {
        if(highlightIndex != null)
        {
            slots[(int)highlightIndex].SetNormal();
            highlightIndex = null;
        }
    }

    public void ReadCommand(int lineIndex)
    {
        CommandType cmd = GetCommandSOFromLineIndex(lineIndex);
        controller.ReleaseCommand(cmd, ProcessManager.Instance.ExecuteTime);

        if(highlightIndex != null)
        {
            slots[(int)highlightIndex].SetNormal();
        }

        highlightIndex = lineIndex;
        slots[lineIndex].SetHighlight();
    }
}

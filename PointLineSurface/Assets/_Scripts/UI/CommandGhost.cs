
using UnityEngine;
using UnityEngine.UI;

public class CommandGhost : MonoBehaviour
{
    [SerializeField] Image icon;
    CommandSlot slot;
    CommandSO commandSO;
    public void Setup(CommandSO commandSO)
    {
        icon.sprite = commandSO.icon;
    }

    public void SetCommandSlot(CommandSlot slot_, CommandSO commandSO_)
    {
        slot = slot_;
        transform.position = slot.transform.position;
        commandSO = commandSO_;
        icon.sprite = commandSO_.icon;
    }

    public void LeftClickDown()
    {
        Command command = DeviceManager.Instance.GetNewCommand(transform);
        command.Setup(commandSO);
        command.DroppedOnSlot(slot);
    }
}

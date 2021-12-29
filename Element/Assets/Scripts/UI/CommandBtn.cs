using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CommandBtn : MonoBehaviour, IPointerDownHandler
{
    public CommandSO commandSO { get; private set; }
    public Command pfCommand;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI keyText;

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        icon.sprite = commandSO.icon;
        keyText.text = commandSO.key.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            Command command = Instantiate(pfCommand, transform.position, Quaternion.identity, OperatorUISystem.Instance.transform);
            command.Setup(commandSO);
            command.BeginDrag();
            OperatorUISystem.Instance.SetCurrentTrackingCommand(command);
        }
    }
}

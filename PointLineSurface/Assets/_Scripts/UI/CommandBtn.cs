using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CommandBtn : PressDownButton
{
    public CommandSO commandSO { get; private set; }
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI keyText;

    public void Setup(CommandSO commandSO_)
    {
        commandSO = commandSO_;
        icon.sprite = commandSO.icon;
        keyText.text = commandSO.key.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
            {
                Transform commandParent = UI_OperateScene.Instance.transform;
                Command command = DeviceManager.Instance.GetNewCommand(commandParent);
                command.transform.position = transform.position;
                command.Setup(commandSO);

/*                ISelectable selectable = command.GetComponent<ISelectable>();
                selectable.LeftClick();
                MouseManager.Instance.SetClickedBeforeDragObj(command.GetComponent<ISelectable>());*/
            }
        }
    }
}

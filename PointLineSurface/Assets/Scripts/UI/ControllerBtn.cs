using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControllerBtn : PressDownButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && MouseManager.Instance.CanClickButton())
            {
                ControllerCreate();
            }
        }
    }

    void ControllerCreate()
    {
        var controller = DeviceManager.Instance.NewController();

        ISelectable selectable = controller.GetComponent<ISelectable>();
        MouseManager.Instance.SetClickedBeforeDragObj(selectable);
    }
}

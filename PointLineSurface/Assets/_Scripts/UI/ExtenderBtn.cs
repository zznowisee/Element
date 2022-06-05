using UnityEngine.EventSystems;

public class ExtenderBtn : DeviceGenerateButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
            {
                CreateExtender();
            }
        }
    }

    void CreateExtender()
    {
        print("Create new extender");
        var extender = DeviceManager.Instance.NewExtender();
        UI_MouseManager.Instance.SetClickedBeforeDragObj(extender);
    }
}

using UnityEngine.EventSystems;

public class MarkBtn : DeviceGenerateButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            if (ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
                CreateMarker();
    }

    private void CreateMarker()
    {
        print("Create new marker.");
        var marker = DeviceManager.Instance.NewMarker();
        UI_MouseManager.Instance.SetClickedBeforeDragObj(marker);
    }
}

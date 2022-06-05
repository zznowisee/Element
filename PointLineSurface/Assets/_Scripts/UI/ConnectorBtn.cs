using UnityEngine.EventSystems;

public class ConnectorBtn : DeviceGenerateButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
            {
                ConnectorCreate();
            }
        }
    }

    void ConnectorCreate()
    {
        var connector = DeviceManager.Instance.NewConnector();
        UI_MouseManager.Instance.SetClickedBeforeDragObj(connector);
    }
}

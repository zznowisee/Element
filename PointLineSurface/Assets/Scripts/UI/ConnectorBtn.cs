using UnityEngine.EventSystems;

public class ConnectorBtn : PressDownButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && MouseManager.Instance.CanClickButton())
            {
                ConnectorCreate();
            }
        }
    }

    void ConnectorCreate()
    {
        var connector = DeviceManager.Instance.NewConnector();

        ISelectable selectable = connector.GetComponent<ISelectable>();
        MouseManager.Instance.SetClickedBeforeDragObj(selectable);
    }
}

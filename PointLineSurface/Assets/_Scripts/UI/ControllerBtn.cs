using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControllerBtn : DeviceGenerateButton
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (ProcessManager.Instance.CanOperate() && UI_MouseManager.Instance.CanClickButton())
            {
                DeviceManager.Instance.GetNewController();
                OnControllerSpawn();
            }
        }
    }

    private void OnControllerSpawn()
    {
        data.levelBuildData.controllerLeftNumber--;
        UpdateVisual();
    }

    public void OnControllerDestory()
    {
        data.levelBuildData.controllerLeftNumber++;
        UpdateVisual();
    }
}

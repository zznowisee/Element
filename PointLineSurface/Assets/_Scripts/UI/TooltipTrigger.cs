
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [TextArea]
    public string content;
    static LTDescr delay;
    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.6f, () =>
        {
            UI_TooltipManager.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        UI_TooltipManager.Hide();
    }
}

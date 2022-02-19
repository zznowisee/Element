
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
            TooltipManager.Show(content, header);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipManager.Hide();
    }
}

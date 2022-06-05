using UnityEngine;
using System;

public class BreakPointConsole : MonoBehaviour
{
    public static event Action OnSettingBreakPoint;
    public static event Action OnCancelSettingBreakPoint;

    static BreakPointSlot current;
    [SerializeField] BreakPointSlot pfBreakPointSlot;
    [SerializeField] Color slotDefaultCol;
    [SerializeField] Color slotSelectCol;
    void Start()
    {
        int slotCount = 45;

        for (int i = 0; i < slotCount; i++)
        {
            BreakPointSlot slot = Instantiate(pfBreakPointSlot, transform);
            slot.Setup(slotSelectCol, slotDefaultCol, i);

            slot.OnBeSelected += BreakPointSlot_OnBeSelected;
            slot.OnCancelSelected += BreakPointSlot_OnCancelSelected;
        }
    }

    void BreakPointSlot_OnCancelSelected()
    {
        current = null;
        OnCancelSettingBreakPoint?.Invoke();
    }

    void BreakPointSlot_OnBeSelected(BreakPointSlot bps_)
    {
        if(current != null)
        {
            current.CancelSelect();
        }
        else
        {
            //first time set break point
            OnSettingBreakPoint?.Invoke();
        }
        current = bps_;
    }

    public static int GetBreakPointIndex()
    {
        if(current != null)
        {
            return current.index;
        }
        else
        {
            return -1;
        }
    }
}

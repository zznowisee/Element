using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BreakPointConsole : MonoBehaviour
{
    public BreakPointSlot current;
    [SerializeField] Button breakPointSwitchBtn;
    [SerializeField] BreakPointSlot pfBreakPointSlot;
    [SerializeField] Color slotDefaultCol;
    [SerializeField] Color slotSelectCol;
    void Start()
    {
        //int slotCount = OperatorUISystem.Instance.consoleSlotCount;

        /*for (int i = 0; i < slotCount; i++)
        {
            BreakPointSlot slot = Instantiate(pfBreakPointSlot, transform);
            slot.Setup(slotSelectCol, slotDefaultCol, i);

            slot.OnBeSelected += BreakPointSlot_OnBeSelected;
            slot.OnCancelSelected += BreakPointSlot_OnCancelSelected;
        }*/

        breakPointSwitchBtn.onClick.AddListener(() =>
        {

        });
    }

    void BreakPointSlot_OnCancelSelected()
    {
        current = null;
    }

    void BreakPointSlot_OnBeSelected(BreakPointSlot bps_)
    {
        if(current != null)
        {
            current.CancelSelect();
        }
        current = bps_;
    }

    public int? GetBreakPointIndex()
    {
        if(current != null)
        {
            return current.index;
        }
        else
        {
            return null;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;

public class BreakPointSlot : MonoBehaviour
{
    bool beSelecting;
    public int index { get; private set; }
    public Image image;
    public event Action<BreakPointSlot> OnBeSelected;
    public event Action OnCancelSelected;
    Color selectCol;
    Color defaultCol;
    public void Setup(Color selectCol_, Color defaultCol_, int index_)
    {
        index = index_;
        selectCol = selectCol_;
        defaultCol = defaultCol_;
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (beSelecting)
            {
                CancelSelect();
                OnCancelSelected?.Invoke();
            }
            else
            {
                BeSelect();
                OnBeSelected?.Invoke(this);
            }
        });
    }

    public void CancelSelect()
    {
        beSelecting = false;
        image.color = defaultCol;
    }

    public void BeSelect()
    {
        beSelecting = true;
        image.color = selectCol;
    }
}

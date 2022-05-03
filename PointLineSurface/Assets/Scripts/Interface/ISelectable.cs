using UnityEngine;

public interface ISelectable
{
    SelectableType type { get; set; }
    bool selected { get; set; }
    bool preSelected { get; set; }
    Transform transform { get; }
    GameObject gameObject { get; }
    Vector3 delta { get; set; }
    void LeftClick();
    void RightClick();
    void Dragging();
    void LeftRelease(out bool destoryed);
    void DraggingWithList(Vector3 position);
}

public enum SelectableType
{
    UI,
    World
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MouseManager : MonoBehaviour
{
    public enum MouseState
    {
        SelectionBox,
        SingleDragging,
        ListDragging,
        Null
    }

    [SerializeField] MouseState mouseState;

    public static UI_MouseManager Instance { get; private set; }

    Camera mainCam;
    public Color color;
    public Sprite selectionBoxSprite;
    public RectTransform selectionMask;
    public CommandGhost commandGhost;
    public CommandSO[] commandSOArray;

    Vector2 startPosition;
    Vector2 endPosition;

    RectTransform boxRect;

    [SerializeField] List<ISelectable> selectableGroupList;
    List<ISelectable> selectingList;
    ISelectable clickedBeforeDrag;
    ISelectable clickedAfterDrag;
    public GameObject debug;
    public event Action<List<ISelectable>> OnSelectionChanged;

    public bool CanClickButton() => !Input.GetKey(KeyCode.LeftShift);

    void Awake()
    {
        Instance = this;
        selectableGroupList = new List<ISelectable>();
        selectingList = new List<ISelectable>();
    }

    void Start()
    {
        mainCam = Camera.main;
        mouseState = MouseState.Null;
        ValidateCanvas();
        CreateBoxRect();
        ResetBoxRect();
    }

    void Update()
    {
        HandleShortcutCommand();
        MousePressDown();
        MouseHeld();
        MouseRelease();
    }

    public void AddISelectableObj(ISelectable selectable)
    {
        selectableGroupList.Add(selectable);
    }

    public void RemoveISelectableObj(ISelectable selectable)
    {
        selectableGroupList.Remove(selectable);
    }

    void HandleShortcutCommand()
    {
        commandGhost.gameObject.SetActive(false);
        foreach(CommandSO commandSO in commandSOArray)
        {
            if (Input.GetKey(commandSO.key))
            {
                CommandSlot slot = InputHelper.GetCommandSlotUnder(InputHelper.MouseWorldPositionIn2D);
                if(slot != null)
                {
                    commandGhost.gameObject.SetActive(true);
                    commandGhost.SetCommandSlot(slot, commandSO);
                    break;
                }
            }
        }
    }

    void ValidateCanvas()
    {
        var canvas = GetComponent<Canvas>();
        if(canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            throw new Exception("SelectionBox component must be placed on a canvas in Screen Space Overlay mode.");
        }

        var canvasScaler = GetComponent<CanvasScaler>();

        if (canvasScaler && canvasScaler.enabled && (!Mathf.Approximately(canvasScaler.scaleFactor, 1f) || canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize))
        {
            Destroy(canvasScaler);
            Debug.LogWarning("SelectionBox component is on a gameObject with a Canvas Scaler component. As of now, Canvas Scalers without the default settings throw off the coordinates of the selection box. Canvas Scaler has been removed.");
        }
    }

    void CreateBoxRect()
    {
        var selectionBox = new GameObject();

        selectionBox.name = "Selection Box";
        selectionBox.transform.parent = transform;

        Image boxImage = selectionBox.AddComponent<Image>();
        boxImage.sprite = selectionBoxSprite;
        boxImage.type = Image.Type.Sliced;
        boxImage.color = color;
        boxRect = selectionBox.transform as RectTransform;
    }

    void ResetBoxRect()
    {
        startPosition = Vector2.zero;
        boxRect.anchoredPosition = Vector2.zero;
        boxRect.sizeDelta = Vector2.zero;
        boxRect.anchorMax = Vector2.zero;
        boxRect.anchorMin = Vector2.zero;
        boxRect.pivot = Vector2.zero;
        boxRect.gameObject.SetActive(false);
    }

    public void SetClickedBeforeDragObj(ISelectable selectable)
    {
        clickedBeforeDrag = selectable;
    }

    void MousePressDown()
    {
        if (GameManager.Instance.GameState != GameState.OperateScene || !ProcessManager.Instance.CanOperate())
            return;
        //clickedBeforeDrag = null;
        if (Input.GetMouseButtonDown(0))
        {
            if (commandGhost.gameObject.activeSelf)
            {
                commandGhost.LeftClickDown();
                ResetSelectableGroupList();
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                BeginSelection();
                return;
            }
            else
            {
                if(clickedBeforeDrag == null) clickedBeforeDrag = GetSelectableAtMousePosition();
                if (clickedBeforeDrag == null)
                {
                    ResetSelectableGroupList();
                    if (!IsPointerClickOnClickableObj())
                    {
                        BeginSelection();
                    }
                    return;
                }
                else
                {
                    if (selectingList.Contains(clickedBeforeDrag))
                    {
                        // need to control all selecting objs
                        foreach(var selectable in selectingList)
                        {
                            selectable.delta = clickedBeforeDrag.transform.position - selectable.transform.position;
                            selectable.LeftClick();
                        }
                        mouseState = MouseState.ListDragging;
                        return;
                    }
                    else
                    {
                        ResetSelectableGroupList();
                        clickedBeforeDrag.LeftClick();
                        clickedBeforeDrag.selected = true;
                        mouseState = MouseState.SingleDragging;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ISelectable selectable = GetSelectableAtMousePosition();
            if(selectable != null)
            {
                selectable.RightClick();
            }
        }
    }

    void MouseHeld()
    {
        if (Input.GetMouseButton(0))
        {
            switch (mouseState)
            {
                case MouseState.ListDragging:
                    clickedBeforeDrag.Dragging();
                    selectingList.ForEach(selectable => selectable.DraggingWithList(clickedBeforeDrag.transform.position - selectable.delta));
                    break;
                case MouseState.SelectionBox:
                    DragSelection();
                    break;
                case MouseState.SingleDragging:
                    clickedBeforeDrag.Dragging();
                    break;
            }
        }
    }

    void MouseRelease()
    {
        if (Input.GetMouseButtonUp(0))
        {
            switch (mouseState)
            {
                case MouseState.ListDragging:
                    for (int i = 0; i < selectingList.Count; i++)
                    {
                        var selectable = selectingList[i];
                        bool isDestoryed = false;
                        selectable.LeftRelease(out isDestoryed);
                        if (isDestoryed)
                        {
                            selectingList.RemoveAt(i);
                            i--;
                        }
                    }
                    clickedBeforeDrag = null;
                    break;
                case MouseState.SelectionBox:
                    EndSelection();
                    break;
                case MouseState.SingleDragging:
                    bool destoryed = false;
                    clickedBeforeDrag.LeftRelease(out destoryed);
                    clickedBeforeDrag = null;
                    break;
            }

            mouseState = MouseState.Null;
        }
    }

    void ResetSelectableGroupList()
    {
        //print("Reset Selectable Group");
        foreach (var selectable in selectableGroupList)
            selectable.selected = false;
        selectingList.Clear();
    }

    void BeginSelection()
    {
        mouseState = MouseState.SelectionBox;
        boxRect.gameObject.SetActive(true);
        startPosition = Input.mousePosition;

        if (!PointIsValidAgainstSelectionMask(startPosition))
        {
            ResetBoxRect();
            return;
        }

        boxRect.anchoredPosition = startPosition;

        //unselected all selecting go if not helding leftshift
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ResetSelectableGroupList();
        }
        else
        {
            ISelectable selectable = GetSelectableAtMousePosition();
            if(selectable != null)
            {
                selectable.preSelected = true;
            }
        }
    }

    void DragSelection()
    {
        if (!boxRect.gameObject.activeSelf)
            return;

        Vector2 currentMousePosition = Input.mousePosition;

        Vector2 lowerLeft = new Vector2(
            Mathf.Min(startPosition.x, currentMousePosition.x),
            Mathf.Min(startPosition.y, currentMousePosition.y));
        Vector2 upperRight = new Vector2(
            Mathf.Max(startPosition.x, currentMousePosition.x),
            Mathf.Max(startPosition.y, currentMousePosition.y));

        boxRect.anchoredPosition = lowerLeft;
        float sizeX = Mathf.Clamp(upperRight.x - lowerLeft.x, 2f, float.MaxValue);
        float sizeY = Mathf.Clamp(upperRight.y - lowerLeft.y, 2f, float.MaxValue);
        boxRect.sizeDelta = new Vector2(sizeX, sizeY);
        foreach(var selectable in selectableGroupList)
        {
            Vector3 screenPoint = GetScreenPointOfSelectable(selectable);

            selectable.preSelected = RectTransformUtility.RectangleContainsScreenPoint(boxRect, screenPoint, null) && PointIsValidAgainstSelectionMask(screenPoint);
        }
    }

    void EndSelection()
    {
        if (!boxRect.gameObject.activeSelf)
            return;

        //clickedAfterDrag = GetSelectableAtMousePosition();
        clickedBeforeDrag = null;
        //ApplySingleClickDeselection();
        ApplyPreSelections();
        ResetBoxRect();
        selectingList = GetAllSelected();
        OnSelectionChanged?.Invoke(selectingList);
    }

    void ApplyPreSelections()
    {
        foreach(var selectable in selectableGroupList)
        {
            if (selectable.preSelected)
            {
                selectable.selected = true;
                selectable.preSelected = false;
            }
        }
    }

    void ApplySingleClickDeselection()
    {
        if (clickedBeforeDrag == null)
            return;

        if(clickedAfterDrag != null && clickedBeforeDrag.preSelected && clickedBeforeDrag.transform == clickedAfterDrag.transform && !Input.GetKey(KeyCode.LeftShift))
        {
            clickedBeforeDrag.selected = false;
            clickedBeforeDrag.preSelected = false;
        }
    }

    bool PointIsValidAgainstSelectionMask(Vector2 screenPoint)
    {
        if (!selectionMask)
            return true;
        Camera screenPointCamera = GetScreenPointCamera(selectionMask);
        return RectTransformUtility.RectangleContainsScreenPoint(selectionMask, screenPoint, screenPointCamera);
    }

    Vector2 GetScreenPointOfSelectable(ISelectable selectable)
    {
        var rectTransform = selectable.transform as RectTransform;

        if (rectTransform)
        {
            return RectTransformUtility.WorldToScreenPoint(mainCam, selectable.transform.position);
        }

        return mainCam.WorldToScreenPoint(selectable.transform.position);
    }

    ISelectable GetSelectableAtMousePosition()
    {
        if (!PointIsValidAgainstSelectionMask(Input.mousePosition))
        {
            return null;
        }

        //UI
        foreach(var selectable in selectableGroupList)
        {
            var rectTransform = (selectable.transform as RectTransform);

            if (rectTransform)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, mainCam))
                {
                    return selectable;
                }
            }
        }
        //WORLD
        RaycastHit2D[] hits = Physics2D.RaycastAll(InputHelper.MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                ISelectable selectable = hits[i].collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    return selectable;
                }
            }
        }

        return null;
    }

    public List<ISelectable> GetAllSelected()
    {
        var selectedList = new List<ISelectable>();
        foreach(var selectable in selectableGroupList)
        {
            if (selectable.selected)
            {
                selectedList.Add(selectable);
            }
        }

        return selectedList;
    }

    Camera GetScreenPointCamera(RectTransform rectTransform)
    {
        Canvas rootCanvas = null;
        RectTransform rectCheck = rectTransform;
        do
        {
            rootCanvas = rectCheck.GetComponent<Canvas>();

            if (rootCanvas && !rootCanvas.isRootCanvas)
            {
                rootCanvas = null;
            }

            rectCheck = (RectTransform)rectCheck.parent;
        } while (!rootCanvas);

        switch (rootCanvas.renderMode)
        {
            case RenderMode.ScreenSpaceCamera:
                return rootCanvas.worldCamera ? rootCanvas.worldCamera : Camera.main;
            case RenderMode.ScreenSpaceOverlay:
                return null;
            default:
            case RenderMode.WorldSpace:
                return Camera.main;
        }
    }

    bool IsPointerClickOnClickableObj()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        foreach(RaycastResult hit in list)
        {
            PressDownButton clickable = hit.gameObject.GetComponent<PressDownButton>();
            Button button = hit.gameObject.GetComponent<Button>();
            if (clickable != null || button != null)
            {
                return true;
            }
        }
        return false;
    }

}

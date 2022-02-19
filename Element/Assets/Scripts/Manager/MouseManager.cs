using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

    Camera mainCam;
    public Color color;
    public Sprite selectionBoxSprite;
    public RectTransform selectionMask;
    public CommandGhost commandGhost;
    public CommandSO[] commandSOArray;

    Vector2 startPosition;
    Vector2 endPosition;

    RectTransform boxRect;

    List<ISelectable> selectableGroupList;
    List<ISelectable> selectingList;
    ISelectable clickedBeforeDrag;
    ISelectable clickedAfterDrag;

    public event Action<List<ISelectable>> OnSelectionChanged;

    void Awake()
    {
        Instance = this;

        selectableGroupList = new List<ISelectable>();
        selectingList = new List<ISelectable>();
    }

    void Start()
    {
        mainCam = Camera.main;
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
                CommandSlot slot = InputHelper.GetCommandSlotUnder(Input.mousePosition);
                if(slot != null)
                {
                    commandGhost.SetCommandSlot(slot, commandSO);
                    commandGhost.gameObject.SetActive(true);
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
        print("Reset Box Rect");
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

    public void ClearClickedBeforeDragObj()
    {
        clickedBeforeDrag = null;
    }

    void MousePressDown()
    {
        if (GameManager.Instance.GameState != GameState.OperatingRoom || !ProcessManager.Instance.CanOperate())
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
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if(clickedBeforeDrag == null)
                {
                    clickedBeforeDrag = GetSelectableAtMousePosition();
                }
                if(clickedBeforeDrag != null)
                {
                    if (selectingList.Contains(clickedBeforeDrag))
                    {
                        foreach(var selectable in selectingList)
                        {
                            selectable.delta = clickedBeforeDrag.transform.position - selectable.transform.position;
                            selectable.LeftClick();
                        }
                    }
                    else
                    {
                        print("clicked before drag is not null");
                        clickedBeforeDrag.LeftClick();
                        print(clickedBeforeDrag.transform.gameObject.name);
                        ResetSelectableGroupList();
                    }
                    return;
                }
                else
                {
                    ResetSelectableGroupList();
                }
            }
            if (!IsPointerClickOnClickableObj())
            {
                //box selection
                BeginSelection();
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
            if(selectingList != null)
            {
                if (selectingList.Count != 0)
                {
                    clickedBeforeDrag.Dragging();
                    foreach (var selectable in selectingList)
                    {
                        selectable.DraggingWithList(clickedBeforeDrag.transform.position - selectable.delta);
                    }
                    return;
                }
            }
            if(clickedBeforeDrag != null)
            {
                clickedBeforeDrag.Dragging();
            }
            else
            {
                DragSelection();
            }
        }
    }

    void MouseRelease()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (selectingList != null)
            {
                if (selectingList.Count != 0)
                {
                    foreach (var selectable in selectingList)
                        selectable.LeftRelease();
                    return;
                }
            }
            if (clickedBeforeDrag != null)
            {
                clickedBeforeDrag.LeftRelease();
                clickedBeforeDrag = null;
            }
            else
            {
                EndSelection();
            }
        }
    }

    void ResetSelectableGroupList()
    {
        foreach (var selectable in selectableGroupList)
            selectable.selected = false;
        selectingList.Clear();
    }

    void BeginSelection()
    {
        print("Begin Selection");
        boxRect.gameObject.SetActive(true);
        startPosition = Input.mousePosition;

        if (!PointIsValidAgainstSelectionMask(startPosition))
        {
            print("Point is Not Valid");
            ResetBoxRect();
            return;
        }

        boxRect.anchoredPosition = startPosition;

        //unselected all selecting go if not helding leftshift
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ResetSelectableGroupList(); ;
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
        boxRect.sizeDelta = upperRight - lowerLeft;

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

        clickedAfterDrag = GetSelectableAtMousePosition();
        
        ApplySingleClickDeselection();
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
                print("Clickable");
                return true;
            }
        }
        print("Unclickable");
        return false;
    }

}

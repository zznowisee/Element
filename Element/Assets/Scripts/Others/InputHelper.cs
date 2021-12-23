using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor;

public static class InputHelper
{
    static Camera mainCamera;
    static Camera MainCamera
    {
        get
        {
            if(mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
    }

    public static Vector2 MouseWorldPositionIn2D
    {
        get
        {
            return MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public static bool IsMouseOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static HexCell GetHexCellUnderPosition3D()
    {
        if (IsMouseOverUIObject())
        {
            return null;
        }

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<HexCell>();
        }

        return null;
    }

    public static IMouseAction GetIMouseDragUnderPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                IMouseAction drag = hits[i].collider.GetComponent<IMouseAction>();
                if (drag != null)
                {
                    return drag;
                }
            }
        }
        return null;
    }

    public static Brush GetBrushUnderPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                Brush brush = hits[i].collider.GetComponent<Brush>();
                if(brush != null)
                {
                    return brush;
                }
            }
        }
        return null;
    }
    public static CommandSlot GetCommandSlotUnderPosition2D()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        for (int i = 0; i < list.Count; i++)
        {
            CommandSlot slot = list[i].gameObject.GetComponent<CommandSlot>();
            if (slot != null)
            {
                return slot;
            }
        }
        return null;
    }

    public static KeyCode HeldKeysInThese(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                return keyCodes[i];
            }
        }
        return KeyCode.Escape;
    }

    public static bool IsTheseKeysDown(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsTheseKeysHeld(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static Direction GetHexDirectionFromAngle(float angle)
    {
        if (angle < 0f) angle += 360f;
        if (angle <= 60f && angle > 0) return Direction.NE;
        else if (angle > 60f && angle <= 120f) return Direction.E;
        else if (angle > 120f && angle <= 180f) return Direction.SE;
        else if (angle > 180f && angle <= 240f) return Direction.SW;
        else if (angle > 240f & angle <= 300f) return Direction.W;
        else return Direction.NW;
    }

    public static int GetIndexFromIndicesArray(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] == 0)
            {
                indices[i] = i + 1;
                return i + 1;
            }
        }

        return 0;
    }

    public static void Save(params Object[] datas)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            EditorUtility.SetDirty(datas[i]);
        }

        AssetDatabase.SaveAssets();
    }
}

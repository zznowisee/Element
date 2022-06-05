using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public static Vector2 GetWorldPosition(Vector3 screenPos)
    {
        return MainCamera.ScreenToWorldPoint(screenPos);
    }

    public static Vector2 GetScreenPosition(Vector3 worldPos)
    {
        return MainCamera.WorldToScreenPoint(worldPos);
    }

    public static bool IsMouseOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static HexCell GetHexCellUnderPosition3D(Vector3 position)
    {
        if (IsPositionOverUIObject(position))
        {
            return null;
        }

        Ray ray = MainCamera.ScreenPointToRay(GetScreenPosition(position));
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<HexCell>();
        }

        return null;
    }

    public static Device GetDeviceUnderPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                Device device = hits[i].collider.GetComponent<Device>();
                if (device != null)
                {
                    return device;
                }
            }
        }
        return null;
    }

    public static bool IsPositionOverUIObject(Vector3 worldPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mainCamera.WorldToScreenPoint(worldPosition);
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);

        for (int i = 0; i < list.Count; i++)
        {
            RectTransform rect = list[i].gameObject.GetComponent<RectTransform>();
            if (rect)
            {
                return true;
            }
        }
        return false;
    }

    public static CommandSlot GetCommandSlotUnder(Vector3 worldPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mainCamera.WorldToScreenPoint(worldPosition);
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

        return -1;
    }
}

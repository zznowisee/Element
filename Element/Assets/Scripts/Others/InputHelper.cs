using UnityEngine;

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
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    public static HexCell GetHexCellUnderPosition3D()
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<HexCell>();
        }

        return null;
    }
    
    public static IMouseDrag GetIMouseDragUnderPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                IMouseDrag drag = hits[i].collider.GetComponent<IMouseDrag>();
                if (drag != null)
                {
                    return drag;
                }
            }
        }
        return null;
    }

    public static FullColorBrush GetColorBrushUnderPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                FullColorBrush brush = hits[i].collider.GetComponent<FullColorBrush>();
                if(brush != null)
                {
                    return brush;
                }
            }
        }
        return null;
    }

    public static WayPoint GetWayPointPosition2D()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MouseWorldPositionIn2D, Vector3.forward, float.MaxValue);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider)
            {
                WayPoint wayPoint = hits[i].collider.GetComponent<WayPoint>();
                if (wayPoint != null)
                {
                    return wayPoint;
                }
            }
        }
        return null;
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

    public static HexDirection GetHexDirectionFromAngle(float angle)
    {
        if (angle < 0f) angle += 360f;
        if (angle <= 60f && angle > 0) return HexDirection.NE;
        else if (angle > 60f && angle <= 120f) return HexDirection.E;
        else if (angle > 120f && angle <= 180f) return HexDirection.SE;
        else if (angle > 180f && angle <= 240f) return HexDirection.SW;
        else if (angle > 240f & angle <= 300f) return HexDirection.W;
        else return HexDirection.NW;
    }

    public static Vector3 GetDirectionFromAngleInHex(float angle)
    {
        if (angle < 0f) angle += 360f;
        if (angle <= 60f && angle > 0) return new Vector3(Mathf.Sin(Mathf.PI / 6f), Mathf.Cos(Mathf.PI / 6f));
        else if (angle > 60f && angle <= 120f) return Vector3.right;
        else if (angle > 120f && angle <= 180f) return new Vector3(Mathf.Sin(Mathf.PI / 6f), -Mathf.Cos(Mathf.PI / 6f));
        else if (angle > 180f && angle <= 240f) return new Vector3(-Mathf.Sin(Mathf.PI / 6f), -Mathf.Cos(Mathf.PI / 6f));
        else if (angle > 240f & angle <= 300f) return Vector3.left;
        else return new Vector3(-Mathf.Sin(Mathf.PI / 6f), Mathf.Cos(Mathf.PI / 6f));
    }

    public static Vector3 GetWireEndPositionFromMouse(Vector3 startPosition, Vector3 dir)
    {
        Vector3 mousePos = MouseWorldPositionIn2D - (Vector2)startPosition;
        if(dir.y == 0)
        {
            return new Vector3(mousePos.x, 0f) + startPosition;
        }
        else
        {
            float k = -dir.x / dir.y;
            float b = mousePos.y - k * mousePos.x;
            float x = b / ((dir.y / dir.x) - k);
            float y = k * x + b;
            return new Vector3(x, y) + startPosition;
        }
    }
}

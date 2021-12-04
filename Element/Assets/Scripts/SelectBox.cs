using UnityEngine;
using UnityEngine.UI;

public class SelectBox : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 endPosition;
    Rect selectionRect;
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();    
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionRect = new Rect();
        }
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            Vector2 center = (startPosition + endPosition) / 2f;
            rectTransform.position = center;
            float sizeX = Mathf.Abs(startPosition.x - endPosition.x);
            float sizeY = Mathf.Abs(startPosition.y - endPosition.y);
            rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}

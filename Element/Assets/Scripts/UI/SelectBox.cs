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
            rectTransform.sizeDelta = Vector2.zero;
            startPosition = Input.mousePosition;
            selectionRect = new Rect();
        }
        else if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            float sizeX = endPosition.x - startPosition.x;
            float sizeY = endPosition.y - startPosition.y;
            rectTransform.anchoredPosition = startPosition + new Vector3(sizeX / 2f, sizeY / 2f);
            rectTransform.sizeDelta = new Vector2(Mathf.Abs(sizeX), Mathf.Abs(sizeY));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            rectTransform.sizeDelta = Vector2.zero;
        }
    }
}

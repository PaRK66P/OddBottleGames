using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObject : MonoBehaviour
{
    [SerializeField]
    private RectTransform _canvasRect;

    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 canvasPosition = new Vector2(
            viewportPosition.x * _canvasRect.sizeDelta.x,
            viewportPosition.y * _canvasRect.sizeDelta.y);

        _rectTransform.anchoredPosition = canvasPosition;
    }
}

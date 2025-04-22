using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorImage;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorImage, new Vector2(0.5f * (cursorImage.width - 1), 0.5f * (cursorImage.height - 1)), CursorMode.Auto);
    }
}

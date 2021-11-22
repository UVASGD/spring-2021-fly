using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Tooltip : MonoBehaviour
{
    public TMP_Text textField;
    private RectTransform rectTransform;
    private Vector2 size;

    public void SetText(string text)
    {
        textField.SetText(text);
    }

    public void EnableTooltip(string text = "")
    {
        gameObject.SetActive(true);
        SetText(text);
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
        SetText("");
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        size = rectTransform.rect.size;
        DisableTooltip();
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseScreenPosition = MouseToScreenSpace(mousePosition);
        
        float rightBoundX = mouseScreenPosition.x + size.x;
        float leftBoundX = mouseScreenPosition.x - size.x;
        float offsetMaxX = Mathf.Max(0f, rightBoundX - Screen.width / 2f);
        float offsetMinX = Mathf.Min(0f, Screen.width / 2f - leftBoundX);
        float offsetX = offsetMinX + offsetMaxX;
        float pivotOffset = offsetX / size.x / 2f;

        transform.position = mousePosition;
        rectTransform.pivot = new Vector2(0.5f + pivotOffset, 2f);
    }

    private Vector2 MouseToScreenSpace(Vector2 mousePosition)
    {
        mousePosition.x -= Screen.width / 2f;
        mousePosition.y -= Screen.height / 2f;
        return mousePosition;
    }
}

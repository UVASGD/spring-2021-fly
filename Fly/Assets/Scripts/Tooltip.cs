using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Tooltip : MonoBehaviour
{
    public TMP_Text textField;

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
        DisableTooltip();
    }

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        transform.position = mousePosition;
        Vector3 rightBound = new Vector3(mousePosition.x + 200f, mousePosition.y, mousePosition.z);
        Vector3 leftBound = new Vector3(mousePosition.x - 200f, mousePosition.y, mousePosition.z);
        float xOffsetMax = Mathf.Max(rightBound.x - Screen.width, 0f);
        float xOffsetMin = Mathf.Min(leftBound.x, 0f);
        float normalizedXOffset = (xOffsetMin + xOffsetMax) / 400f + 0.5f;
        GetComponent<RectTransform>().pivot = new Vector2(normalizedXOffset, 2);
    }

    private Vector2 NormalizeScreenPosition(Vector2 position)
    {
        return new Vector2(position.x / Screen.width, position.y / Screen.height);
    }
}

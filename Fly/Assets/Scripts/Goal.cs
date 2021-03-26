using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public RectTransform uiTransform;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        uiTransform.anchoredPosition = screenPos;
        //uiTransform.position = transform.position;
    }
}

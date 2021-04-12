using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICloud : MonoBehaviour
{

    private RectTransform rectTransform;
    private Image image;

    [SerializeField]
    [Tooltip("How many seconds to cross the screen completely.")]
    private float screenPeriodMean = 5f;
    [SerializeField]
    private float screenPeriodDeviation = 3f;
    private float screenPeriod;
    [SerializeField]
    private bool leftToRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        CalculateScreenPeriod();
    }

    void CalculateScreenPeriod()
    {
        screenPeriod = screenPeriodMean + Random.Range(-screenPeriodDeviation, screenPeriodDeviation);
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition += Vector2.right * (750f * (leftToRight ? 1 : -1) / screenPeriod * Time.deltaTime);

        float paddedWidthExtents = 750f / 2f * 1.2f;
        float paddedHeightExtents = 600f / 2f * 0.8f;
        if (leftToRight)
        {
            if (rectTransform.anchoredPosition.x > paddedWidthExtents)
            {
                rectTransform.anchoredPosition = new Vector2(-paddedWidthExtents, Random.Range(-paddedHeightExtents, paddedHeightExtents));
                CalculateScreenPeriod();
            }
        }
        else
        {
            if (rectTransform.anchoredPosition.x < -paddedWidthExtents)
            {
                rectTransform.anchoredPosition = new Vector2(paddedWidthExtents, Random.Range(-paddedHeightExtents, paddedHeightExtents));
                CalculateScreenPeriod();
            }
        }
    }
}

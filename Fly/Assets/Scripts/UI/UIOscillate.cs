using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOscillate : MonoBehaviour
{
    [Header("Position")]
    [Range(0f, 10f)]
    public float xAmplitude;
    [Range(0f, 10f)]
    public float xFrequency;
    [Range(-1f, 1f)]
    public float xPeriodOffset;
    [Range(0f, 10f)]
    public float yAmplitude;
    [Range(0f, 10f)]
    public float yFrequency;
    [Range(-1f, 1f)]
    public float yPeriodOffset;

    [Header("Rotation")]
    [Range(0f, 2f)]
    public float rotateFrequency;
    [Range(0f, 90f)]
    public float rotateAmplitude;
    [Range(-1f, 1f)]
    public float rotatePeriodOffset;

    [Header("Scale")]
    [Range(0f, 2f)]
    public float scaleFrequency;
    [Range(0f, 5f)]
    public float scaleAmplitude;
    [Range(-1f, 1f)]
    public float scalePeriodOffset;

    private RectTransform rectTransform;
    private bool activated;
    private Vector2 startPos;
    private Vector2 startScale;
    private float startRot;
    private Vector2 offset;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Activate()
    {
        print(name + " activated!");
        activated = true;
        startPos = rectTransform.anchoredPosition;
        startRot = rectTransform.eulerAngles.z;
        startScale = rectTransform.localScale;
        time = 0f;
    }

    public void Deactivate()
    {
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated) return;
        
        // Position
        offset.x = Mathf.Sin((2 * Mathf.PI * xFrequency + Mathf.PI * xPeriodOffset) * time) * xAmplitude;
        offset.y = Mathf.Sin((2 * Mathf.PI * yFrequency + Mathf.PI * yPeriodOffset) * time) * yAmplitude;
        rectTransform.anchoredPosition = startPos + offset;

        // Rotation
        rectTransform.eulerAngles = new Vector3(0f, 0f,
            startRot + Mathf.Sin((2 * Mathf.PI * rotateFrequency + Mathf.PI * rotatePeriodOffset) * time) * rotateAmplitude);

        // Scale
        rectTransform.localScale = (Vector3)startScale + Vector3.one * 
            Mathf.Sin((2 * Mathf.PI * scaleFrequency + Mathf.PI * scalePeriodOffset) * time) * scaleAmplitude;

        // Scale

        time += Time.deltaTime;
    }
}

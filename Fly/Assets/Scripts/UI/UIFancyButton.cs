using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFancyButton : MonoBehaviour
{

    public Image image;
    public Button button;
    private Material material;
    public bool highlighting;
    public float t;
    
    [SerializeField]
    [Range(0f, 1f)]
    private float highlightDuration = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        material = image.material;
        highlighting = false;
        t = 0f;
    }

    private void Update()
    {
        if (highlighting)
        {

            t += Time.deltaTime / highlightDuration;
        }
        else
        {

            t -= Time.deltaTime / highlightDuration;
        }
        t = Mathf.Clamp01(t);
        SetHighlight(t);
    }

    public void StartHighlight()
    {
        highlighting = true;
    }

    public void StopHighlight()
    {
        highlighting = false;
    }

    private void SetHighlight(float t)
    {
        material.SetFloat("_Blend", t);
    }

    private void OnMouseEnter()
    {
        StartHighlight();
    }

    private void OnMouseExit()
    {
        StopHighlight();
    }
}

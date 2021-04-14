using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public AnimationCurve tweenCurve;
    public float tweenDuration;
    public Image fade;
    public float fadeDuration;
    public List<GameObject> panels;

    public int defaultPanelIndex;
    private int lastIndex;
    private bool transitioning;

    public bool enableEscape;
    public bool menuOpen;

    public List<SavableUI> savables;

    private void Start()
    {
        instance = this;
        lastIndex = 0;
        transitioning = false;
        menuOpen = enableEscape ? false : true;

        foreach (SavableUI item in savables)
        {
            item.Load();
        }

        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == defaultPanelIndex && menuOpen ? true : false);
        }
    }

    private void Update()
    {
        if (enableEscape && !transitioning && Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOpen)
            {
                menuOpen = false;
                FadeOut();
            }
            else
            {
                menuOpen = true;
                FadeIn(defaultPanelIndex);
            }
        }
    }

    public void SetActivePanel(int index)
    {
        if (transitioning) return;
        if (index >= panels.Count) return;
        StartCoroutine(CrossFade(index));
    }


    public void FadeOut()
    {
        if (transitioning) return;
        StartCoroutine(FadeOutCR());
    }

    IEnumerator FadeOutCR()
    {
        transitioning = true;

        float t = 0f;
        float tSmooth = t;
        GameObject panel = panels[lastIndex];
        panel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            panel.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        panel.SetActive(false);
        menuOpen = false;
        transitioning = false;
    }

    public void FadeIn(int index)
    {
        if (transitioning) return;
        if (index >= panels.Count) return;
        StartCoroutine(FadeInCR(index));
    }

    IEnumerator FadeInCR(int index)
    {
        transitioning = true;

        float t = 0f;
        float tSmooth = t;
        GameObject panel = panels[index];
        panel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            panel.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        panel.SetActive(true);
        lastIndex = index;
        menuOpen = true;
        transitioning = false;
    }

    IEnumerator CrossFade(int index)
    {
        transitioning = true;

        float t = 0f;
        float tSmooth = t;
        GameObject oldPanel = panels[lastIndex];
        GameObject newPanel = panels[index];
        oldPanel.SetActive(true);
        newPanel.SetActive(true);
        while (t < 1f)
        {
            tSmooth = tweenCurve.Evaluate(t);
            oldPanel.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, tSmooth);
            newPanel.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, tSmooth);
            t += Time.deltaTime / tweenDuration;
            yield return new WaitForEndOfFrame();
        }
        newPanel.SetActive(true);
        oldPanel.SetActive(false);
        lastIndex = index;
        transitioning = false;
    }
}

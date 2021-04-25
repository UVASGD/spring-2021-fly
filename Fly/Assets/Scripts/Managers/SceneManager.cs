using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    // Singleton (ensure that only one exists)
    public static SceneManager instance;

    [Header("Scene")]
    public int currentSceneBuildIndex;

    [Header("Transition")]
    [Range(0.1f, 5f)] [SerializeField] private float duration = 1f;
    [SerializeField] private TransitionDirection direction;
    [SerializeField] private TransitionType type;
    [SerializeField] private AnimationCurve tween;
    [SerializeField] private Image transitionImage;
    private RectTransform transitionTransform;
    private bool transitioning;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    public enum TransitionDirection
    {
        LeftRight,
        RightLeft,
    }

    public enum TransitionType
    {
        Slide,
        RadialWipe,
        Fade,
    }

    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentSceneBuildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        transitionTransform = transitionImage.GetComponent<RectTransform>();
        transitionTransform.gameObject.SetActive(false);
    }

    public void LoadNextScene()
    {
        int currentBuildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentBuildIndex + 1) % sceneCount;
        LoadScene(nextSceneIndex);
    }

    public void LoadScene(string sceneName)
    {
        if (transitioning) return;
        int buildIndex = UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(sceneName);
        print($"Loading {sceneName} at buildIndex {buildIndex}");
        LoadScene(buildIndex);
    }

    public void LoadScene(int buildIndex)
    {
        if (transitioning) return;
        transitioning = true;
        if (type == TransitionType.Slide)
            StartCoroutine(LoadSceneSlide(buildIndex));
        else if (type == TransitionType.RadialWipe)
            StartCoroutine(LoadSceneRadialWipe(buildIndex));
        else if (type == TransitionType.Fade)
            StartCoroutine(LoadSceneFade(buildIndex));
    }

    private IEnumerator LoadSceneSlide(int buildIndex)
    {
        transitionTransform.gameObject.SetActive(true);
        float t = 0f;
        int direction = (this.direction == TransitionDirection.LeftRight ? -1 : 1);
        float xMax = Screen.width / 2 + transitionTransform.sizeDelta.x;

        // Assumes RectTransform has a center anchor on the canvas
        Vector2 start = Vector2.right * xMax * direction;
        Vector2 finish = Vector2.right * xMax * -direction;

        transitionImage.type = Image.Type.Simple;

        // Interpolate between start and midpoint at speed of transition
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionTransform.anchoredPosition = Vector3.Lerp(start, Vector3.zero, tweenedT);
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex); // Load the new scene
        currentSceneBuildIndex = buildIndex;
        
        t = 0f;
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionTransform.anchoredPosition = Vector3.Lerp(Vector3.zero, finish, tweenedT); // lerp to midpoint
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }
        transitionTransform.gameObject.SetActive(false);
        transitioning = false;
    }

    private IEnumerator LoadSceneRadialWipe(int buildIndex)
    {
        transitionTransform.gameObject.SetActive(true);
        transitionTransform.anchoredPosition = Vector2.zero;

        float t = 0f;
        bool clockwise = (this.direction == TransitionDirection.LeftRight ? true : false);

        transitionImage.type = Image.Type.Filled;
        transitionImage.fillClockwise = clockwise;

        // Interpolate between start and midpoint at speed of transition
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionImage.fillAmount = Mathf.Lerp(0f, 1f, tweenedT);
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex); // Load the new scene
        currentSceneBuildIndex = buildIndex;

        t = 0f;
        transitionImage.fillClockwise = !clockwise;
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionImage.fillAmount = Mathf.Lerp(1f, 0f, tweenedT);
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }
        transitionTransform.gameObject.SetActive(false);
        transitioning = false;
    }

    private IEnumerator LoadSceneFade(int buildIndex)
    {
        transitionTransform.gameObject.SetActive(true);
        transitionTransform.anchoredPosition = Vector2.zero;

        float t = 0f;
        Color start = transitionImage.color;
        start.a = 0f;
        Color end = start;
        start.a = 1f;

        // Interpolate between start and end colors
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionImage.color = Color.Lerp(start, end, tweenedT);
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }

        if (buildIndex < 0)
        {
            Quit(immediate: true);
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex); // Load the new scene
        currentSceneBuildIndex = buildIndex;

        t = 0f;
        while (t < 1f)
        {
            float tweenedT = tween.Evaluate(t); // Applies tween to lerp. AKA smooth~~~
            transitionImage.fillAmount = Mathf.Lerp(1f, 0f, tweenedT);
            transitionImage.color = Color.Lerp(end, start, tweenedT);
            t += Time.deltaTime / duration; // t goes from 0 to 1 linearly over the duration
            yield return new WaitForEndOfFrame(); // Stop coroutine until next frame
        }
        transitionTransform.gameObject.SetActive(false);
        transitioning = false;
    }

    private void Update()
    {
        if (!debugMode) return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadNextScene();
        }
    }

    public void LoadLevel(int level)
    {
        LoadScene(level);
    }

    public void Quit(bool immediate = false)
    {
        if (immediate)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBGL
            #else
            Application.Quit();
            #endif
        }
        else
        {
            type = TransitionType.Fade;
            LoadScene(-1);
        }
    }
}

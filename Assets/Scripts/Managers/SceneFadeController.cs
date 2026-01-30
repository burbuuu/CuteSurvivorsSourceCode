using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneFadeController : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    
    #region Scene Transition

    private IEnumerator SceneTransition(string sceneName)
    {
        // Pause gameplay
        Time.timeScale = 0f;

        // Fades out loads level and then fades in
        yield return FadeOut();
        
        // Use Async to ensure the scene is actually loaded before Fading In
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (op is { isDone: false })
        {
            yield return null;
        }
        
        yield return FadeIn();

        // Resume gameplay
        Time.timeScale = 1f;
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;  // Timescale is set to 0 during scene transitions
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;  // Timescale is set to 0 during scene transitions
            fadeCanvas.alpha = 1f - t / fadeDuration;
            yield return null;
        }
    }

    #endregion

    #region Scene transition Public API 

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(SceneTransition(sceneName));
    }

    #endregion
};


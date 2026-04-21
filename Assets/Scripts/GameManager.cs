using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene  = "MainMenu";
    [SerializeField] private string gameScene      = "GameScene";
    [SerializeField] private string gameOverScene  = "GameOver";

    [Header("Fade Overlay")]
    [SerializeField] private Image fadeImage;  
    [SerializeField] private float fadeDuration = 0.4f;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
            StartCoroutine(FadeIn());
    }

    public void StartGame()
    {
        LoadScene(gameScene);
    }

    public void GameOver()
    {
        LoadScene(gameOverScene);
    }

    public void RetryGame()
    {
        LoadScene(gameScene);
    }

    public void GoToMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }
}
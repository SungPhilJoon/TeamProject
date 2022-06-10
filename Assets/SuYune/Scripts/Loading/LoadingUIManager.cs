using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingUIManager : Singleton<LoadingUIManager>
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image progressBar;

    // private string loadSceneName = "Start_Scene";
    // private static LoadingUIController instance;
    // public static LoadingUIController Instance
    // {
    //     get
    //     {
    //         if (instance == null)
    //         {
    //             var obj = FindObjectOfType<LoadingUIController>();
    //             if (obj != null)
    //             {
    //                 instance = obj;
    //             }
    //             else
    //             {
    //                 instance = Create();
    //             }
    //         }
    //         return instance;
    //     }
    // }
    // 
    // private static LoadingUIController Create()
    // {
    //     return Instantiate(Resources.Load<LoadingUIController>("LoadingUI"));
    // }

    // private void Awake()
    // {
    //     if (Instance != this)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     DontDestroyOnLoad(gameObject);
    // }

    public void LoadScene()
    {
        canvasGroup.gameObject.SetActive(true);
        // SceneManager.sceneLoaded += OnSceneLoaded;
        // loadSceneName = sceneName;
        StartCoroutine(LoadSceneProgress());
    }

    private IEnumerator LoadSceneProgress()
    {
        progressBar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        // AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        // op.allowSceneActivation = false;

        float timer = 0f;
        while (true)
        {
            yield return null;
            
            timer += Time.unscaledDeltaTime;
            progressBar.fillAmount = Mathf.Lerp(0.0f, 1f, timer/3);

            if (progressBar.fillAmount >= 1f)
            {
                // op.allowSceneActivation = true;
                yield return StartCoroutine(Fade(false));
                yield break;
            }

        }
    }

    // 씬 로딩이 끝난 시점을 알려줌
    // private void OnSceneLoaded(Scene arg0)
    // {
    //     if (arg0.name == loadSceneName)
    //     {
    //         StartCoroutine(Fade(false));
    //         //SceneManager.sceneLoaded -= OnSceneLoaded;
    //     }
    // }

    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = isFadeIn ? 1f : Mathf.Lerp(1f, 0f, timer);
        }

        if (!isFadeIn)
        {
            canvasGroup.gameObject.SetActive(false);
        }
    }
}
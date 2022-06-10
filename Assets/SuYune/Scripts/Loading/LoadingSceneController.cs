using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// LoadingSceneController_Int.LoadScene(1);

public class LoadingSceneController : MonoBehaviour
{
    #region Variables
    static int nextSceneNum = 1;

    [SerializeField]
    private GameObject progressBarObject;
    [SerializeField]
    private Image progressBar;

    private FadeEffect fadeEffect;

    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        // Time.timeScale = 0f;
        fadeEffect = GetComponent<FadeEffect>();
    }

    private void Start()
    {
        // StartCoroutine(LoadSceneProgress());
    }

    #endregion Unity Methods

    #region Helper Methods
    public void OnClickStart()
    {
        fadeEffect.fadeImage.gameObject.SetActive(true);
        fadeEffect.background.gameObject.SetActive(true);

        progressBarObject.SetActive(true);

        // Time.timeScale = 1f;
        StartCoroutine(LoadSceneProgress());
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public static void LoadScene(int sceneNum)
    {
        nextSceneNum = sceneNum;
        // SceneManager.LoadScene("LoadingScene");
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneNum); // LoadSceneAsync: 비동기 방식으로 씬을 불러오는 도중에 다른 작업이 가능
        op.allowSceneActivation = false;

        float timer = 0f;
        
        while(!op.isDone)
        {
            yield return null;

            if (op.progress < 0.8f)     // 로딩바의 80%까지는 로딩진행도에 따라서 진행바가 채워짐
            {
                progressBar.fillAmount = op.progress;
            }
            else        // 로딩바의 남은 10%는 1초간 채운 뒤 씬을 불러옴
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.8f, 1f, timer/3);
                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        
    }

    #endregion Helper Methods
}

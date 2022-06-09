using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// LoadingSceneController_Int.LoadScene(1);

public class LoadingSceneController : MonoBehaviour
{
    static int nextSceneNum = 1;

    [SerializeField]
    private Image progressBar;
    [SerializeField]
    private Image startImage;

    public static void LoadScene(int sceneNum)
    {
        nextSceneNum = sceneNum;
        // SceneManager.LoadScene("LoadingScene");
        SceneManager.LoadScene(1);
    }

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    private void Start()
    {
        startImage.gameObject.SetActive(true);
        // StartCoroutine(LoadSceneProgress());
    }

    public void OnClickStart()
    {
        startImage.gameObject.SetActive(false);
        Time.timeScale = 1f;
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
                progressBar.fillAmount = Mathf.Lerp(0.8f, 1f, timer/10);
                if(progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        
    }
}

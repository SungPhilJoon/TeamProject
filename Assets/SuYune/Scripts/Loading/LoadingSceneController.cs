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
        Application.Quit(); // ���ø����̼� ����
#endif
    }


    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneNum); // LoadSceneAsync: �񵿱� ������� ���� �ҷ����� ���߿� �ٸ� �۾��� ����
        op.allowSceneActivation = false;

        float timer = 0f;
        
        while(!op.isDone)
        {
            yield return null;

            if (op.progress < 0.8f)     // �ε����� 80%������ �ε����൵�� ���� ����ٰ� ä����
            {
                progressBar.fillAmount = op.progress;
            }
            else        // �ε����� ���� 10%�� 1�ʰ� ä�� �� ���� �ҷ���
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

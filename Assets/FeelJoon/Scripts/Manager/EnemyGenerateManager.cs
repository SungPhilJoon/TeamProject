using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;
using ETeam.KyungSeo;

public class EnemyGenerateManager : Singleton<EnemyGenerateManager>
{
    #region Variables
    [SerializeField] private float updateDelay = 0.2f;

    [SerializeField] private float generateDelay = 2f;

    #endregion Variables
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<EnemyController>().GenerateHandler = GenerateEnemyDelay;
        }

        StartCoroutine(UpdateDelay(updateDelay));
    }

    #region Helper Methods
    public void EnemyGenerate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                GameObject enemy = transform.GetChild(i).gameObject;

                StartCoroutine(enemy.GetComponent<EnemyController>().GenerateHandler(enemy, generateDelay));
                break;
            }
        }
    }

    private IEnumerator UpdateDelay(float updateDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(updateDelay);

            EnemyGenerate();
        }
    }

    private IEnumerator GenerateEnemyDelay(GameObject enemy, float generateDelay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < generateDelay)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        enemy.SetActive(true);
    }

    #endregion Helper Methods
}

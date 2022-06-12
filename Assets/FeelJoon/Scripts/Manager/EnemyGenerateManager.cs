using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;
using UnityChanAdventure.KyungSeo;

public class EnemyGenerateManager : Singleton<EnemyGenerateManager>
{
    #region Variables
    [SerializeField] private float updateDelay = 0.2f;

    [SerializeField] private float generateDelay = 2f;

    private EnemyController[] enemies;

    #endregion Variables

    #region Properties
    public EnemyController[] Enemies => enemies;

    #endregion Properties

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        enemies = new EnemyController[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            enemies[i] = transform.GetChild(i).GetComponent<EnemyController>();
        }
    }

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            enemies[i].GenerateHandler = GenerateEnemyDelay;
        }

        StartCoroutine(UpdateDelay(updateDelay));
    }

    #endregion Unity Methods

    #region Helper Methods
    public void EnemyGenerate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.activeSelf)
            {
                StartCoroutine(enemies[i].GenerateHandler(enemies[i].gameObject, generateDelay));
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

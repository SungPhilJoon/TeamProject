using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.KyungSeo;
using ETeam.FeelJoon;
using ETeam.YongHak;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Actions
    [NonSerialized] public Action<float> GenerateMonsterUpdate;

    #endregion Actions

    #region Variables
    [SerializeField] private GameObject player;

    private MainPlayerController mainPlayer;

    public GameObject unavailableSkillText;
    public GameObject reviveParticle;

    public Camera mainCamera;

    private Shop shop;

    private Vector3 revivePosition;

    [SerializeField] private Text goldAmountText;

    private readonly int Dead = Animator.StringToHash("IsPlayerDead");

    public bool isCharacterEnterBossGround = false;

    #endregion Variables

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;
        mainPlayer = player.GetComponent<MainPlayerController>();

        shop = FindObjectOfType<Shop>();
    }

    void Start()
    {
        revivePosition = mainPlayer.reviveTransform.position;

        StartCoroutine(UpdateGoldAmount());
    }

    #endregion Unity Methods

    #region Properties
    public GameObject Player => player;

    public MainPlayerController Main => mainPlayer;

    public EnemyController[] Enemies => EnemyGenerateManager.Instance.Enemies;

    public bool IsPlayerDead => !mainPlayer.IsAlive;

    #endregion Properties

    /// <summary>
    /// 부활 구현 함수
    /// </summary>
    public void Revive()
    {
        mainPlayer.controller.enabled = false;
        mainPlayer.transform.position = revivePosition;
        GameObject reviveObj = Instantiate(reviveParticle, revivePosition, Quaternion.identity);
        Destroy(reviveObj, 2f);
        mainPlayer.controller.enabled = true;

        StatsObject playerStats = mainPlayer.playerStats;

        playerStats.AddHealth(playerStats.GetModifiedValue(CharacterAttribute.Health));
        playerStats.AddMana(playerStats.GetModifiedValue(CharacterAttribute.Mana));

        mainPlayer.StateMachine.ChangeState<PlayerIdle>();
        mainPlayer.animator.ResetTrigger("OnDeadTrigger");
        mainPlayer.animator.SetBool("IsAlive", true);

        mainPlayer.gameoverUI.SetActive(false);

        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].EnemyAnimator.SetBool(Dead, false);
            Enemies[i].StateMachine.ChangeState<EnemyIdleState>();
        }
    }

    private IEnumerator UpdateGoldAmount()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (goldAmountText != null)
            {
                goldAmountText.text = mainPlayer.gold.ToString("#,###");

            }
            
            if (shop != null)
            {
                shop.coinText.text = mainPlayer.gold.ToString("#,###");
            }
        }
    }
}

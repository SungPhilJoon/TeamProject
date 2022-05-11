using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.KyungSeo;
using ETeam.FeelJoon;
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

    private readonly int Dead = Animator.StringToHash("IsPlayerDead");

    #endregion Variables

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        mainPlayer = player.GetComponent<MainPlayerController>();
    }

    #endregion Unity Methods

    #region Properties
    public GameObject Player => player;

    public EnemyController[] Enemies => EnemyGenerateManager.Instance.Enemies;

    public bool IsPlayerDead => !mainPlayer.IsAlive;

    #endregion Properties

    /// <summary>
    /// 부활 구현 함수
    /// </summary>
    public void Revive()
    {
        mainPlayer.controller.enabled = false;
        mainPlayer.transform.position = mainPlayer.reviveTransform.position;
        mainPlayer.controller.enabled = true;

        mainPlayer.health = mainPlayer.maxHealth;

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
}

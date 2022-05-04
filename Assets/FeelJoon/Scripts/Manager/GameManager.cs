using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;
using ETeam.FeelJoon;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Actions
    [NonSerialized] public Action<float> GenerateMonsterUpdate;

    #endregion Actions

    #region Variables
    [SerializeField] private GameObject player = GameObject.FindGameObjectWithTag("Player");

    [SerializeField] private MainPlayerController mainPlayer;

    [SerializeField] private EnemyController[] enemies = FindObjectsOfType<EnemyController>();

    private readonly int Dead = Animator.StringToHash("IsPlayerDead");

    #endregion Variables

    #region Properties
    public GameObject Player => player;

    public EnemyController[] EnemyController => enemies;
    
    public bool IsPlayerDead => !player.gameObject.activeSelf;

    #endregion Properties

    public void Revive() // ���� �߻���. ��?
    {
        mainPlayer = player.GetComponent<MainPlayerController>();

        mainPlayer.controller.enabled = false;
        mainPlayer.transform.position = mainPlayer.reviveTransform.position;
        mainPlayer.controller.enabled = true;

        mainPlayer.health = mainPlayer.maxHealth;

        mainPlayer.StateMachine.ChangeState<PlayerIdle>();
        mainPlayer.animator.ResetTrigger("OnDeadTrigger");
        mainPlayer.animator.SetBool("IsAlive", true);

        // mainPlayer.PlayerInput.SwitchCurrentActionMap("Default");
        mainPlayer.gameoverUI.SetActive(false);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].EnemyAnimator.SetBool(Dead, false);
            enemies[i].StateMachine.ChangeState<EnemyIdleState>();
        }
    }
}

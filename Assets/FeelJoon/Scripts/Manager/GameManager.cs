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
<<<<<<< Updated upstream
    private GameObject player = GameObject.FindGameObjectWithTag("Player");
    // 아이템 적용 등 체력에 접근하려면 플레이어 컨트롤러가 필요해보여서 넣어봤습니다.
    private MainPlayerController mainPlayer = GameObject.FindObjectOfType<MainPlayerController>();
    private EnemyController[] enemyController = GameObject.FindObjectsOfType<EnemyController>();
    private static readonly int Dead = Animator.StringToHash("IsPlayerDead");
=======
    [SerializeField] private GameObject player = GameObject.FindGameObjectWithTag("Player");

    [SerializeField] private EnemyController[] enemies;
>>>>>>> Stashed changes

    #endregion Variables

    #region Properties
    public GameObject Player => player;

    public EnemyController[] EnemyController => enemyController;
    
    public bool IsPlayerDead => !player.gameObject.activeSelf;

    #endregion Properties

    public void Revive() // 버그 발생중. 롸?
    {
        mainPlayer = player.GetComponent<MainPlayerController>();
        mainPlayer.controller.enabled = false;
        mainPlayer.transform.position = mainPlayer.reviveTransform.position;
        mainPlayer.controller.enabled = true;
        mainPlayer.health = mainPlayer.maxHealth;
        //playerController.animator.Rebind();
        mainPlayer.StateMachine.ChangeState<PlayerIdle>();
        mainPlayer.animator.ResetTrigger("OnDeadTrigger");
        mainPlayer.animator.SetBool("IsAlive", true);
        mainPlayer.playerInput.SwitchCurrentActionMap("Default");
        mainPlayer.gameoverUI.SetActive(false);

        for (int i = 0; i < enemyController.Length; i++)
        {
            enemyController[i].EnemyAnimator.SetBool(Dead,false);
            enemyController[i].StateMachine.ChangeState<EnemyIdleState>();
        }
    }
}

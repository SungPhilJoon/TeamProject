using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;
using ETeam.FeelJoon;

public class GameManager : Singleton<GameManager>
{
    #region Variables
    private GameObject player = GameObject.FindGameObjectWithTag("Player");
    // ������ ���� �� ü�¿� �����Ϸ��� �÷��̾� ��Ʈ�ѷ��� �ʿ��غ����� �־�ý��ϴ�.
    private MainPlayerController mainPlayer = GameObject.FindObjectOfType<MainPlayerController>();
    private EnemyController[] enemyController = GameObject.FindObjectsOfType<EnemyController>();
    private static readonly int Dead = Animator.StringToHash("IsPlayerDead");

    #endregion Variables

    #region Properties
    public GameObject Player => player;

    public EnemyController[] EnemyController => enemyController;
    
    public bool IsPlayerDead => !player.gameObject.activeSelf;

    #endregion Properties

    public void Revive() // ���� �߻���. ��?
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

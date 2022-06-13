using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;
using UnityChanAdventure.FeelJoon;
using UnityChanAdventure.YongHak;
using System;
using System.Linq;
using Random = UnityEngine.Random;

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

    [HideInInspector] public Camera mainCamera;

    private Shop shop;

    private Vector3 revivePosition;

    [SerializeField] private Text goldAmountText;

    private readonly int Dead = Animator.StringToHash("IsPlayerDead");

    [Header("보스 몬스터들")]
    public BossController[] bossEnemies;

    [SerializeField] private AudioClip villageBGM;

    #endregion Variables

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;
        mainPlayer = player.GetComponent<MainPlayerController>();

        shop = FindObjectOfType<Shop>();

        mainCamera.transform.localPosition = Vector3.forward * -11f;
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

    #region Helper Methods
    /// <summary>
    /// 부활 구현 함수
    /// </summary>
    public void PlayerRespawn()
    {
        GameObject reviveObj = Instantiate(reviveParticle, revivePosition, Quaternion.identity);
        Destroy(reviveObj, 2f);

        StatsObject playerStats = mainPlayer.playerStats;

        playerStats.AddHealth(playerStats.GetModifiedValue(CharacterAttribute.Health));
        playerStats.AddMana(playerStats.GetModifiedValue(CharacterAttribute.Mana));

        mainPlayer.StateMachine.ChangeState<PlayerIdle>();
        mainPlayer.animator.ResetTrigger("OnDeadTrigger");
        mainPlayer.animator.SetBool("IsAlive", true);

        mainPlayer.gameoverUI.SetActive(false);

        if (isPlayerEnterBossGround())
        {
            foreach (BossController bossEnemy in bossEnemies)
            {
                if (bossEnemy.isPlayerEnterBossGround)
                {
                    bossEnemy.isPlayerEnterBossGround = false;
                    bossEnemy.bossHPBarObj.SetActive(false);
                    break;
                }
            }
        }

        for (int i = 0; i < Enemies.Length; i++)
        {
            Enemies[i].EnemyAnimator.SetBool(Dead, false);
            Enemies[i].StateMachine.ChangeState<EnemyIdleState>();
        }

        AudioManager.Instance.ChangeBGMAndPlayDelay(villageBGM, 0.1f);
        AudioManager.Instance.PlayForceSFX(
        AudioManager.Instance.playerSFXAudioSource,
        AudioManager.Instance.playerSFXClips,
        "PlayerRevive");
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

    public bool isPlayerEnterBossGround()
    {
        return bossEnemies.Any(boss => boss.isPlayerEnterBossGround == true);
    }

    public void CameraVibrateEffect(float intensity)
    {
        mainCamera.transform.localPosition = new Vector3(Random.insideUnitCircle.x * intensity, Random.insideUnitCircle.y * intensity, mainCamera.transform.localPosition.z);
    }

    #endregion Helper Methods
}

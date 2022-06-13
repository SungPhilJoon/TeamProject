using UnityChanAdventure.FeelJoon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController), typeof(NavMeshAgent))]
public partial class BossController : MonoBehaviour, IAttackable, IDamageable
{
    #region Variables
    protected StateMachine<BossController> stateMachine;

    private Animator animator;

    public LayerMask targetMask;
    public EnemyType enemyType;

    internal BossFieldOfView bossFOV;

    public NavMeshAgent agent;

    [Header("보스 체력")]
    public int maxHealth;
    public int health;
    public Image bossHPBar;
    public Text bossHPText;

    [HideInInspector] public GameObject bossHPBarObj;

    [Header("보스 전투")]
    public int bossPhase = 1;
    public int damage = 50;
    public int increaseDamageAmount = 1;
    public float coolTime;
    public float meleeAttackCoolDown = 3f;
    public float throwAttackCoolDown = 7f;
    public GameObject attackPlaceArea;
    public GameObject jumpAttackPlaceArea;
    public int exp;
    //public float jumpSitAttackCoolDown = 10f;

    private ManualCollision bossManualCollision;

    [Header("투사체")]
    public WeaponThrow bossWeapon;
    public Transform throwPoint;

    [Header("드랍 아이템 목록")]
    [SerializeField] private ItemObjectDatabase[] database;

    [Header("보스 ID")]
    public int bossID;

    [Header("보스 데미지 텍스트")]
    [SerializeField] private NPCBattleUI battleUI;

    [Header("Boss Gates")]
    [SerializeField] private GateController enterGate;
    [SerializeField] private GateController exitGate;
    [HideInInspector] public bool isPlayerEnterBossGround = false;

    [Header("Drop ItemObject List")]
    [SerializeField] private ItemObject[] dropItemObjects;

    public Transform hitTransform;

    private Transform projectilePoint;

    private Dictionary<int, Func<float, IEnumerator>> skillCoolTimeHandlers = new Dictionary<int, Func<float, IEnumerator>>();

    public float targetDistance;

    protected readonly int hashAttackDistance = Animator.StringToHash("AttackDistance");

    #endregion Variables

    #region Properties
    public Transform ProjectilePoint
    {
        get => projectilePoint;
    }

    public bool IsAvailableMeleeAttack
    {
        get => (targetDistance < 5 && targetDistance > 0);
    }

    public bool IsAvailableThrowAttack
    {
        get => (targetDistance >= 5);
    }

    public Transform Target => bossFOV.Target;

    public ManualCollision BossManualCollision => bossManualCollision;

    public float HealthPercentage => Health / (float)maxHealth;

    private int Health
    {
        get => health;
    }

    public int FinalDamage => damage * increaseDamageAmount;

    #endregion Properties

    #region Unity Methods
    void Awake()
    {
        bossFOV = GetComponent<BossFieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        bossManualCollision = GetComponentInChildren<ManualCollision>();
    }

    void Start()
    {
        stateMachine = new StateMachine<BossController>(this, new BossIdle());
        stateMachine.AddState(new BossRun());
        stateMachine.AddState(new BossAngry());
        stateMachine.AddState(new BossMeleeAttack());
        stateMachine.AddState(new BossThrowAttack());
        stateMachine.AddState(new BossJumpAndSit());
        stateMachine.AddState(new BossDead());

        animator = GetComponent<Animator>();

        skillCoolTimeHandlers.Add(BossSkillNameList.BossMeleeAttack1_Name.GetHashCode(), SkillCoolTime);
        skillCoolTimeHandlers.Add(BossSkillNameList.BossThrowAttack1_Name.GetHashCode(), SkillCoolTime);
        skillCoolTimeHandlers.Add(BossSkillNameList.BossThrowAttack2_Name.GetHashCode(), SkillCoolTime);

        health = maxHealth;

        if (battleUI != null)
        {
            battleUI.MinimumValue = 0;
            battleUI.MaximumValue = maxHealth;
            battleUI.Value = Health;
        }

        enterGate.OnEnterGate -= OnEnterBossGate;
        enterGate.OnEnterGate += OnEnterBossGate;
        exitGate.OnEnterGate -= OnExitBossGate;
        exitGate.OnEnterGate += OnExitBossGate;

        bossHPBarObj = bossHPBar.transform.parent.parent.gameObject;
    }

    void Update()
    {
        stateMachine.Update(Time.deltaTime);
    }

    #endregion Unity Methods

    #region IAttackable
    public AttackBehaviour CurrentAttackBehaviour => throw new System.NotImplementedException();

    public void OnExecuteMeleeAttack()
    {
        AudioManager.Instance.PlaySFX(
        AudioManager.Instance.enemySFXAudioSource,
        AudioManager.Instance.enemySFXClips,
        "BossAttackEffect");

        bossManualCollision.CheckCollision();

        foreach (Collider targetCollider in bossManualCollision.targetColliders)
        {
            if (targetCollider == null)
            {
                continue;
            }

            if (targetCollider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(FinalDamage);
            }
        }
    }

    public void OnExecuteProjectileAttack()
    {
        bossWeapon.owner = this;
        GameObject obj = Instantiate(bossWeapon.gameObject, throwPoint.position, Quaternion.identity);
        obj.transform.parent = null;

        GameObject areaObj = GameObject.Instantiate(attackPlaceArea,
            transform.position + transform.forward * 30f,
            transform.rotation);
        areaObj.transform.localScale += Vector3.forward * 5f;

        GameObject.Destroy(areaObj, 1.5f);
    }

    #endregion IAttackable

    #region IDamageable
    public bool IsAlive => Health > 0;

    public void TakeDamage(int damage, Transform target = null, GameObject hitEffectPrefab = null)
    {
        if (!IsAlive)
        {
            return;
        }

        if (hitEffectPrefab)
        {
            if (hitTransform != null)
            {
                Instantiate(hitEffectPrefab, hitTransform.position, Quaternion.identity);
            }
        }

        health -= damage;

        if (bossHPBar != null && bossHPText != null)
        {
            bossHPBar.fillAmount = HealthPercentage;

            if (health <= 0)
            {
                bossHPText.text = $"0/{maxHealth}";
            }
            else
            {
                bossHPText.text = $"{health}/{maxHealth}";
            }
        }

        if (battleUI != null)
        {
            battleUI.Value = health;
            battleUI.CreateDamageText(damage);
        }

        if (!IsAlive)
        {
            for (int i = 0; i < dropItemObjects.Length; i++)
            {
                DropItem(dropItemObjects[i]);
            }
            DropGold();

            stateMachine.ChangeState<BossDead>();

            QuestManager.Instance.ProcessQuest(QuestType.DestroyEnemy, bossID);

            GameManager.Instance.Main.playerStats.AddExp(exp);

            return;
        }

        if (HealthPercentage < 0.5f && bossPhase.Equals(1))
        {
            bossPhase = 2;
            coolTime = 2f;
            stateMachine.ChangeState<BossAngry>();
        }
    }

    #endregion IDamageable

    #region Helper Methods
    private IEnumerator SkillCoolTime(float skillCoolTime)
    {
        float normalTime = 0f;

        while (skillCoolTime > normalTime)
        {
            normalTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void DropItem(ItemObject dropItemObject)
    {
        dropItemObject.data = dropItemObject.CreateItem();

        GameObject dropItem = new GameObject();
        dropItem.layer = LayerMask.NameToLayer("Interactable");

        Vector2 randomItemPosition = Random.insideUnitCircle * 3f;
        dropItem.transform.position = new Vector3(randomItemPosition.x + transform.position.x,
            transform.position.y + 1f,
            randomItemPosition.y + transform.position.z);

        SpriteRenderer itemImage = dropItem.AddComponent<SpriteRenderer>();
        itemImage.sprite = dropItemObject.icon;

        SphereCollider itemCollider = dropItem.AddComponent<SphereCollider>();
        itemCollider.radius = 0.3f;

        dropItem.AddComponent<PickupItem>().itemObject = dropItemObject;
        dropItem.AddComponent<CameraFacing>();

        Destroy(dropItem, 10f);
    }

    private void DropGold()
    {
        GameManager.Instance.Main.gold += 100000;
    }

    private void OnEnterBossGate()
    {
        AudioManager.Instance.enemySFXAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("EnemySFXVolume", 0f);

        StartCoroutine(SwitchFlag());
        bossHPBarObj.SetActive(true);
    }

    private void OnExitBossGate()
    {
        AudioManager.Instance.enemySFXAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("EnemySFXVolume", -30f);

        //StopCoroutine(SwitchFlag());
        isPlayerEnterBossGround = false;
        bossHPBarObj.SetActive(false);
    }

    private IEnumerator SwitchFlag()
    {
        yield return new WaitForSeconds(4f);

        isPlayerEnterBossGround = true;
    }

    #endregion Helper Methods
}

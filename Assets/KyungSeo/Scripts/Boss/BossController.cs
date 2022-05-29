using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ETeam.FeelJoon;
using System;

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

    [Header("보스 전투")]
    public int bossPhase = 1;
    public int damage = 50;
    public int increaseDamageAmount = 1;
    public float coolTime;
    public float meleeAttackCoolDown = 3f;
    public float throwAttackCoolDown = 7f;
    //public float jumpSitAttackCoolDown = 10f;

    private ManualCollision bossManualCollision;

    [Header("투사체")]
    public WeaponThrow bossWeapon;
    public Transform throwPoint;

    [Header("드랍 아이템 목록")]
    [SerializeField] private ItemObjectDatabase[] database;

    public Transform hitTransform;

    private Transform projectilePoint;

    private Dictionary<int, Func<float, IEnumerator>> skillCoolTimeHandlers = new Dictionary<int, Func<float, IEnumerator>>();

    private int minRandomGoldAmount;
    private int maxRandomGoldAmount;

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
    public Transform Target => bossFOV.target;

    public float HealthPercentage => health / (float)maxHealth;

    public int FinalDamage => damage * increaseDamageAmount;

    #endregion Properties

    #region Unity Methods
    void Awake()
    {
        minRandomGoldAmount = maxHealth / 5;
        maxRandomGoldAmount = maxHealth;

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
        //animator.SetFloat(hashAttackDistance, targetDistance);

        skillCoolTimeHandlers.Add(BossSkillNameList.BossMeleeAttack1_Name.GetHashCode(), SkillCoolTime);
        skillCoolTimeHandlers.Add(BossSkillNameList.BossThrowAttack1_Name.GetHashCode(), SkillCoolTime);
        skillCoolTimeHandlers.Add(BossSkillNameList.BossThrowAttack2_Name.GetHashCode(), SkillCoolTime);

        health = maxHealth;
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
        bossManualCollision.CheckCollision();

        foreach(Collider targetCollider in bossManualCollision.targetColliders)
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
    }

    #endregion IAttackable

    #region IDamageable
    public bool IsAlive => health > 0;

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

        if (!IsAlive)
        {
            stateMachine.ChangeState<BossDead>();
            return;
        }

        if (HealthPercentage < 0.5f && bossPhase.Equals(1))
        {
            bossPhase = 2;
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

    #endregion Helper Methods

}

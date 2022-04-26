using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public enum EnemyType
    {
        Melee,
        Projectile,
        Boss,
    }

    /// <summary>
    /// EnemyController�� �⺻ ����Ÿ Ŭ���� �Դϴ�.
    /// �ٰŸ��� ���Ÿ� Enemy�� �� Ŭ������ ��� �޾ƾ� �մϴ�. 
    /// </summary>
    public class EnemyController : MonoBehaviour, IAttackable, IDamageable
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;

        protected Animator animator;

        protected ObjectPoolManager<Projectile> objectPoolManager;

        // private Transform target;
        public LayerMask targetMask;
        public EnemyType enemyType;

        public FieldOfView enemyFOV;

        public float attackRange;

        [Header("ü��")]
        public int maxHealth = 100;
        public int health;

        [Header("����")]
        public int damage;
        public float coolTime;
        private ManualCollision enemyManualCollision;

        public Transform projectilePoint;

        protected readonly int hashHitTrigger = Animator.StringToHash("Hit");

        [SerializeField] private NPCBattleUI battleUI;

        #endregion Variables

        #region Properties
        public StateMachine<EnemyController> StateMachine => stateMachine;

        public Transform Target => enemyFOV.target;

        public bool IsAvailableAttack
        {
            get
            {
                if (Target == null || !Target.GetComponent<IDamageable>().IsAlive)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.transform.position);
                return (distance <= attackRange);
            }
        }

        #endregion Properties

        #region Unity Methods
        protected virtual void Awake()
        {
            if (enemyType == EnemyType.Melee)
            {
                return;
            }

            objectPoolManager = new ObjectPoolManager<Projectile>(PooledObjectNameList.NameOfProjectile, projectilePoint);
        }

        protected virtual void Start()
        {
            stateMachine = new StateMachine<EnemyController>(this, new EnemyIdleState());
            stateMachine.AddState(new EnemyMoveState());
            stateMachine.AddState(new EnemyAttackState());
            stateMachine.AddState(new EnemyDeadState());
            stateMachine.AddState(new EnemyVictoryState());

            animator = GetComponent<Animator>();

            health = maxHealth;

            enemyFOV = GetComponent<FieldOfView>();

            enemyManualCollision = GetComponentInChildren<ManualCollision>();

            if (battleUI != null)
            {
                battleUI.MinimumValue = 0;
                battleUI.MaximumValue = maxHealth;
                battleUI.Value = health;
            }
        }

        protected virtual void Update()
        {
            if (GameManager.Instance.IsPlayerDead)
            {
                stateMachine.ChangeState<EnemyVictoryState>();
                return;
            }

            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region IAttackable
        public AttackBehaviour CurrentAttackBehaviour => throw new System.NotImplementedException();

        public void OnExecuteMeleeAttack()
        {
            enemyManualCollision.CheckCollision();

            foreach (Collider targetCollider in enemyManualCollision.targetColliders)
            {
                if (targetCollider == null)
                {
                    continue;
                }

                IDamageable damageable = targetCollider.GetComponent<IDamageable>();
                damageable.TakeDamage(damage);
            }
        }

        public void OnExecuteProjectileAttack()
        {
            Projectile projectile = objectPoolManager.GetPooledObject(PooledObjectNameList.NameOfProjectile);
            projectile.gameObject.SetActive(true);
            projectile.transform.position = projectilePoint.position;
            projectile.transform.forward = (Target.position - projectilePoint.position).normalized;
            projectile.moveSpeed = 5f;
        }

        #endregion IAttackable

        #region IDamageable
        public bool IsAlive => health > 0;

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (battleUI != null)
            {
                battleUI.Value = health;
                battleUI.CreateDamageText(damage);
            }

            if (IsAlive) 
            {
                animator.SetTrigger(hashHitTrigger);
            }
            else
            {
                stateMachine.ChangeState<EnemyDeadState>();
            }
        }

        #endregion IDamageable
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public enum EnemyType
    {
        Melee,
        Projectile,
        Boss,
    }

    /// <summary>
    /// EnemyController의 기본 데이타 클래스 입니다.
    /// 근거리와 원거리 Enemy는 이 클래스를 상속 받아야 합니다. 
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

        [Header("체력")]
        public int maxHealth = 100;
        public int health;

        [Header("전투")]
        public int damage;
        public float coolTime;

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

            animator = GetComponent<Animator>();

            health = maxHealth;

            enemyFOV = GetComponent<FieldOfView>();

            if (battleUI != null)
            {
                battleUI.MinimumValue = 0;
                battleUI.MaximumValue = maxHealth;
                battleUI.Value = health;
            }
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        public Transform SearchEnemy()
        {
            // Target = null;

            // Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            // if (targetInViewRadius.Length <= 0)
            // {
            //     return null;
            // }

            // Target = targetInViewRadius[0].transform;

            return Target;
        }

        #endregion Helper Methods

        #region IAttackable
        public AttackBehaviour CurrentAttackBehaviour => throw new System.NotImplementedException();

        public void OnExecuteMeleeAttack()
        {
            IDamageable damageable = Target.GetComponent<IDamageable>();
            damageable.TakeDamage(damage);
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

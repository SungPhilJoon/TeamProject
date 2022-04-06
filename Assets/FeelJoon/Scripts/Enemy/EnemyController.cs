using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    /// <summary>
    /// EnemyController의 기본 데이타 클래스 입니다.
    /// 근거리와 원거리 Enemy는 이 클래스를 상속 받아야 합니다. 
    /// </summary>
    public class EnemyController : MonoBehaviour, IAttackable, IDamageable
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;

        protected Animator animator;

        private Transform target;
        public float viewRadius;
        public LayerMask targetMask;

        public float attackRange;

        [Header("체력")]
        public int maxHealth = 100;
        public int health;

        // [SerializeField] private NPCBattleUI battleUI;

        #endregion Variables

        #region Properties
        public StateMachine<EnemyController> StateMachine => stateMachine;

        public Transform Target => target;

        public bool IsAvailableAttack
        {
            get
            {
                if (target == null)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, target.transform.position);
                return (distance <= attackRange);
            }
        }

        #endregion Properties

        #region Unity Methods
        protected virtual void Start()
        {
            stateMachine = new StateMachine<EnemyController>(this, new EnemyIdleState());
            stateMachine.AddState(new EnemyMoveState());
            stateMachine.AddState(new EnemyAttackState());
            stateMachine.AddState(new EnemyHitState());
            stateMachine.AddState(new EnemyDeadState());

            animator = GetComponent<Animator>();

            health = maxHealth;
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        public Transform SearchEnemy()
        {
            target = null;

            Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            if (targetInViewRadius.Length <= 0)
            {
                return null;
            }

            target = targetInViewRadius[0].transform;

            return target;
        }

        #endregion Helper Methods

        #region IAttackable
        public void OnExecuteAttack()
        {
            throw new System.NotImplementedException();
        }

        #endregion IAttackable

        #region IDamageable
        public bool IsAlive => health > 0;

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            throw new System.NotImplementedException();
        }

        #endregion IDamageable
    }
}

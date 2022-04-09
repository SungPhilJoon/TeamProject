using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class PlayerController : MonoBehaviour, IAttackable, IDamageable
    {
        #region Variables
        protected StateMachine<PlayerController> stateMachine;

        [HideInInspector]
        public Animator animator;

        private Transform target;
        public LayerMask targetMask;

        public Transform hitTransform;

        protected bool isMove;

        [Header("Ã¼·Â")]
        public int maxHealth = 100;
        public int health;

        protected readonly int hashSwapIndex = Animator.StringToHash("SwapIndex");
        protected readonly int hashIsMove = Animator.StringToHash("IsMove");
        protected readonly int hashDashTrigger = Animator.StringToHash("DashTrigger");
        protected readonly int hashHitTrigger = Animator.StringToHash("HitTrigger");

        #endregion Variables

        #region Properties
        public StateMachine<PlayerController> StateMachine => stateMachine;

        public Transform Target => target;

        public bool IsMove
        {
            get => isMove;
            set => isMove = value;
        }

        #endregion Properties

        #region Unity Methods
        protected virtual void Awake()
        {
            stateMachine = new StateMachine<PlayerController>(this, new PlayerIdle());
            stateMachine.AddState(new PlayerMove());
            stateMachine.AddState(new PlayerDash());
            stateMachine.AddState(new PlayerAttack());
            stateMachine.AddState(new PlayerDead());

            animator = GetComponentInChildren<Animator>();

            isMove = false;

            health = maxHealth;
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region IAttackable
        public AttackBehaviour CurrentAttackBehaviour => throw new System.NotImplementedException();

        public void OnExecuteAttack()
        {
            
        }

        #endregion IAttackable

        #region IDamageable
        public bool IsAlive => health > 0;

        public void TakeDamage(int damage, GameObject hitEffectPrefab = null)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitTransform);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hashHitTrigger);
            }
            else
            {
                stateMachine.ChangeState<PlayerDead>();
            }
        }

        #endregion IDamageable
    }
}

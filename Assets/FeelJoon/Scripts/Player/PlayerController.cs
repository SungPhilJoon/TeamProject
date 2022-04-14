using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;
using System;

namespace ETeam.FeelJoon
{
    public enum PlayerStance
    {
        Default,
        Sword,
        Bow,
    }

    public class PlayerController : MonoBehaviour, IAttackable, IDamageable
    {
        #region Variables
        protected StateMachine<PlayerController> stateMachine;

        [HideInInspector]
        public Animator animator;
        public PlayerStance playerStance;
        public float attackRange;

        protected Transform target;
        public LayerMask targetMask;

        protected int damage;

        public Transform hitTransform;

        protected bool isMove;

        [Header("Ã¼·Â")]
        public int maxHealth = 100;
        public int health;

        protected readonly int hashSwapIndex = Animator.StringToHash("SwapIndex");
        protected readonly int hashHitTrigger = Animator.StringToHash("HitTrigger");

        #endregion Variables

        #region Properties
        public StateMachine<PlayerController> StateMachine => stateMachine;

        public Transform Target
        {
            get => target;

            set => target = value;
        }

        public float AttackRange
        {
            get => attackRange;

            set => attackRange = CurrentAttackBehaviour?.range ?? 6.0f;
        }

        public virtual int Damage
        {
            get;
            protected set;
        }

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

            playerStance = PlayerStance.Default;

            health = maxHealth;
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        

        #endregion Helper Methods

        #region IAttackable
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            protected set;
        }

        public void OnExecuteAttack()
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            damageable.TakeDamage(damage);
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

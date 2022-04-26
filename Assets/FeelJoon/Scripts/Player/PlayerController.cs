using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.KyungSeo;
using System;

namespace ETeam.FeelJoon
{
    public enum PlayerWeapon
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
        public PlayerWeapon playerWeapon;
        public float attackRange;

        protected Transform target;
        public LayerMask targetMask;

        protected int damage;

        public Transform hitTransform;

        protected bool isMove;

        [Header("체력")]
        public int maxHealth = 100;
        public int health;

        [Header("캐릭터 UI")]
        [SerializeField] private float closeupSpeed;
        [SerializeField] private RawImage characterFace;
        [SerializeField] private Transform characterFaceCameraTransform;

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

            playerWeapon = PlayerWeapon.Default;

            health = maxHealth;
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator ChangePlayerUIColor()
        {
            Color originColor = characterFace.color;
            characterFace.color = Color.red;
            yield return StartCoroutine(CloseUpPlayer());
            characterFace.color = originColor;
        }

        private IEnumerator CloseUpPlayer()
        {
            while (characterFaceCameraTransform.localPosition.z > 0.8)
            {
                characterFaceCameraTransform.localPosition -= new Vector3(0f, 0f, Time.deltaTime * closeupSpeed);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            while (characterFaceCameraTransform.localPosition.z < 1.6)
            {
                characterFaceCameraTransform.localPosition += new Vector3(0f, 0f, Time.deltaTime * closeupSpeed);
                yield return null;
            }

            yield return null;
        }

        #endregion Helper Methods

        #region IAttackable
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            protected set;
        }

        public void OnExecuteMeleeAttack()
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            damageable.TakeDamage(damage);
        }

        public void OnExecuteProjectileAttack()
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
                // animator?.SetTrigger(hashHitTrigger);
                StopAllCoroutines();
                StartCoroutine(ChangePlayerUIColor());
            }
            else
            {
                stateMachine.ChangeState<PlayerDead>();
            }
        }

        #endregion IDamageable
    }
}

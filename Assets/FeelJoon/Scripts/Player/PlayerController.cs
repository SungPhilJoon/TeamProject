using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        public PlayerWeapon currentPlayerWeapon;

        protected Transform target;
        public LayerMask targetMask;

        protected int damage;

        public Transform hitTransform;

        public GameObject hitEffectPrefab;

        protected bool isMove;

        private ManualCollision playerManualCollision;

        [Header("골드")]
        public int gold;

        [Header("플레이어 Stats")]
        public StatsObject playerStats;

        [Header("체력")]
        public int maxHealth = 100;
        public int health;

        [Header("캐릭터 UI")]
        [SerializeField] private float closeupSpeed;
        [SerializeField] private RawImage characterFace;
        [SerializeField] private Transform characterFaceCameraTransform;
        public GameObject gameoverUI;
        [HideInInspector] public TMP_Text gameoverText;

        public Transform reviveTransform;

        protected readonly int hashSwapIndex = Animator.StringToHash("SwapIndex");
        protected readonly int hashHitTrigger = Animator.StringToHash("HitTrigger");

        private Color originColor;

        #endregion Variables

        #region Properties
        public StateMachine<PlayerController> StateMachine => stateMachine;

        public Transform Target
        {
            get => target;

            set => target = value;
        }

        public virtual int Damage
        {
            get; set;
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

            currentPlayerWeapon = PlayerWeapon.Default;

            originColor = characterFace.color;

            maxHealth = playerStats.GetModifiedValue(CharacterAttribute.Health);
            playerStats.Health = maxHealth;

            gameoverUI.SetActive(false);
            gameoverText = gameoverUI.GetComponentInChildren<TMP_Text>();

            playerManualCollision = GetComponentInChildren<PlayerManualCollision>();
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator ChangePlayerUIColor()
        {
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
            playerManualCollision.CheckCollision();

            foreach (Collider targetCollider in playerManualCollision.targetColliders)
            {
                if (targetCollider == null)
                {
                    continue;
                }

                IDamageable damageable = targetCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage, null, hitEffectPrefab);
                }
            }
        }

        public void OnExecuteProjectileAttack()
        {

        }

        #endregion IAttackable

        #region IDamageable
        public bool IsAlive => playerStats.Health > 0;

        public void TakeDamage(int damage, Transform target = null, GameObject hitEffectPrefab = null)
        {
            if (!IsAlive)
            {
                return;
            }

            playerStats.Health = playerStats.AddHealth(-damage);

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitTransform);
            }

            if (IsAlive)
            {
                StopCoroutine(ChangePlayerUIColor());
                StartCoroutine(ChangePlayerUIColor());
            }
            else
            {
                stateMachine.ChangeState<PlayerDead>();
                gameoverUI.SetActive(true);
            }
        }
        #endregion IDamageable
    }
}

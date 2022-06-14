using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;
// using System;

namespace UnityChanAdventure.FeelJoon
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

        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterController controller;
        public PlayerWeapon currentPlayerWeapon;

        protected Transform target;
        public LayerMask targetMask;

        protected int damage;
        [SerializeField] private int criticalRate;

        public Transform hitTransform;

        public GameObject hitEffectPrefab;

        protected bool isMove;

        private ManualCollision playerManualCollision;

        [Header("골드")]
        [HideInInspector] public int gold;

        [Header("플레이어 Stats")]
        public StatsObject playerStats;

        [Header("체력")]
        [HideInInspector] public int maxHealth = 100;
        public int health;

        [Header("마나")]
        [HideInInspector] public int maxMana = 100;
        public int mana;

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

        public int Damage
        {
            get
            {
                if (damage <= 0)
                {
                    damage = playerStats.GetModifiedValue(CharacterAttribute.Strength) / 2;
                }

                int criticalAttackRange = Random.Range(0, 100);

                int finalDamage = damage + playerStats.level * 10;

                if (criticalRate > criticalAttackRange)
                {
                    return finalDamage * 2;
                }

                return finalDamage;
            }
            set => damage = value;
        }

        public int CriticalRate
        {
            get => criticalRate;
            set => criticalRate = value;
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
            controller = GetComponent<CharacterController>();
            isMove = false;

            currentPlayerWeapon = PlayerWeapon.Default;

            originColor = characterFace.color;

            playerStats.SetBaseValue(CharacterAttribute.Health, health);
            maxHealth = playerStats.GetModifiedValue(CharacterAttribute.Health);
            playerStats.Health = maxHealth;

            playerStats.SetBaseValue(CharacterAttribute.Mana, mana);
            maxMana = playerStats.GetModifiedValue(CharacterAttribute.Mana);
            playerStats.Mana = maxMana;

            gameoverUI.SetActive(false);
            gameoverText = gameoverUI.GetComponentInChildren<TMP_Text>();

            playerManualCollision = GetComponentInChildren<PlayerManualCollision>();

            gold = playerStats.gold;
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        protected virtual void OnApplicationQuit()
        {
            playerStats.gold = gold;
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator ChangePlayerUIColor()
        {
            characterFace.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            characterFace.color = originColor;
        }

        private IEnumerator GameOverUISetActiveWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            gameoverUI.SetActive(true);

            yield return null;

            controller.enabled = false;
            transform.position = reviveTransform.position;
            controller.enabled = true;
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
                    damageable.TakeDamage(Damage, null, hitEffectPrefab);
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

            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.playerSFXAudioSource,
            AudioManager.Instance.playerSFXClips,
            "PlayerTakeDamage");

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitTransform);
            }

            if (IsAlive)
            {
                characterFace.color = originColor;
                StopCoroutine(ChangePlayerUIColor());
                StartCoroutine(ChangePlayerUIColor());
            }
            else
            {
                AudioManager.Instance.PlayForceSFX(
                AudioManager.Instance.playerSFXAudioSource,
                AudioManager.Instance.playerSFXClips,
                "PlayerDead");

                stateMachine.ChangeState<PlayerDead>();
                StartCoroutine(GameOverUISetActiveWithDelay(5.0f));

                isMove = false;
            }
        }
        #endregion IDamageable
    }
}

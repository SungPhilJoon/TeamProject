using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public enum EnemyType
    {
        Melee,
        Projectile,
        Boss,
    }

    /// <summary>
    /// 몬스터의 데이터와 행동을 제어하는 클래스
    /// </summary>
    public class EnemyController : MonoBehaviour, IAttackable, IDamageable
    {
        #region Delegates
        public System.Func<GameObject, float, IEnumerator> GenerateHandler;

        #endregion Delegates

        #region Variables
        protected StateMachine<EnemyController> stateMachine;

        protected Animator animator;

        public Projectile projectile;

        public LayerMask targetMask;
        public EnemyType enemyType;

        [HideInInspector]
        public FieldOfView enemyFOV;

        public float attackRange;

        [Header("몬스터 ID")]
        public int enemyID;

        [Header("몬스터 체력")]
        public int maxHealth = 100;
        public int health;

        [Header("몬스터 전투")]
        public int damage;
        public float coolTime;
        public int exp;
        [SerializeField] private AudioClip enemyDeadClip;
        private ManualCollision enemyManualCollision;

        [Header("드랍 아이템 목록")]
        [SerializeField] private ItemObjectDatabase[] database;

        [Header("아이템 드랍 확률")]
        [SerializeField] private float dropItemPercentage;
        private float rndNumberWithDropItem;

        private Transform projectilePoint; // 몬스터가 투사체를 발사하는 위치
        private Vector3 generatePosition; // 몬스터가 다시 생성되는 위치
        private AudioSource audioSource;

        protected readonly int hashHitTrigger = Animator.StringToHash("Hit");

        [SerializeField] private NPCBattleUI battleUI;

        public Transform hitTransform;

        private int minRandomGoldAmount;
        private int maxRandomGoldAmount;

        private bool isCalledStartMethod = false;

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

        public Transform ProjectilePoint
        {
            get => projectilePoint;

            set
            {
                if (enemyType == EnemyType.Melee)
                {
                    projectilePoint = null;
                }

                if (enemyType == EnemyType.Projectile)
                {
                    projectilePoint = value;
                }
            }
        }

        public Animator EnemyAnimator => animator;

        #endregion Properties

        #region Unity Methods
        protected virtual void Awake()
        {
            minRandomGoldAmount = (int)(maxHealth * 0.2f);
            maxRandomGoldAmount = maxHealth;

            if (enemyType == EnemyType.Melee)
            {
                return;
            }

            projectilePoint = transform.GetChild(transform.childCount - 1);
        }

        protected virtual void OnEnable()
        {
            rndNumberWithDropItem = Random.Range(0f, 100f);

            if (isCalledStartMethod)
            {
                stateMachine.ChangeState<EnemyIdleState>();

                transform.position = generatePosition;

                health = maxHealth;
                battleUI.Value = health;

                enemyFOV.target = null;
            }
        }

        protected virtual void Start()
        {
            stateMachine = new StateMachine<EnemyController>(this, new EnemyIdleState());
            stateMachine.AddState(new EnemyMoveState());
            stateMachine.AddState(new EnemyAttackState());
            stateMachine.AddState(new EnemyDeadState());
            stateMachine.AddState(new EnemyVictoryState());

            animator = GetComponent<Animator>();

            audioSource = GetComponent<AudioSource>();

            health = maxHealth;

            enemyFOV = GetComponent<FieldOfView>();

            enemyManualCollision = GetComponentInChildren<ManualCollision>();

            if (battleUI != null)
            {
                battleUI.MinimumValue = 0;
                battleUI.MaximumValue = maxHealth;
                battleUI.Value = health;
            }

            generatePosition = transform.position;

            isCalledStartMethod = true;
        }

        protected virtual void Update()
        {
            if (health > 0 && GameManager.Instance.IsPlayerDead)
            {
                stateMachine.ChangeState<EnemyVictoryState>();
            }

            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private void DropItem()
        {
            int rndItemDatabaseNumber = Random.Range(0, database.Length);

            ItemObject dropItemObject;

            if (rndItemDatabaseNumber.Equals(2))
            {
                dropItemObject = database[rndItemDatabaseNumber].
                itemObjects[Random.Range(0, database[rndItemDatabaseNumber].itemObjects.Length)];
            }
            else
            {
                dropItemObject = database[rndItemDatabaseNumber].
                itemObjects[Random.Range(0, database[rndItemDatabaseNumber].itemObjects.Length - 1)];
            }

            dropItemObject.data = dropItemObject.CreateItem();

            GameObject dropItem = new GameObject();
            dropItem.layer = LayerMask.NameToLayer("Interactable");

            Vector2 randomItemPosition = Random.insideUnitCircle * 0.2f;
            dropItem.transform.position = new Vector3(randomItemPosition.x + transform.position.x, 
                0.5f + transform.position.y, 
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
            GameManager.Instance.Main.gold += Random.Range(minRandomGoldAmount, maxRandomGoldAmount);
        }

        #endregion Helper Methods

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
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }

        public void OnExecuteProjectileAttack()
        {
            projectile.gameObject.SetActive(true);

            projectile.transform.position = projectilePoint.position;

            projectile.moveSpeed = 15f;
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

            health -= damage;

            if (audioSource != null)
            {
                AudioManager.Instance.PlaySFX(
                AudioManager.Instance.enemySFXAudioSource, 
                AudioManager.Instance.enemySFXClips, 
                "EnemyHit");
            }

            if (hitEffectPrefab)
            {
                GameObject obj = Instantiate(hitEffectPrefab, hitTransform.position, Quaternion.identity);
                Destroy(obj, 1f);
            }

            if (battleUI != null)
            {
                battleUI.Value = health;
                battleUI.CreateDamageText(damage);
            }

            if (IsAlive)
            {
                animator.SetTrigger(hashHitTrigger);

                if (Target == null)
                {
                    enemyFOV.FindTakeDamagedTarget(target);
                }
            }
            else
            {
                if (rndNumberWithDropItem < dropItemPercentage)
                {
                    DropItem();
                }

                DropGold();
                stateMachine.ChangeState<EnemyDeadState>();

                AudioManager.Instance.PlayForceSFX(AudioManager.Instance.enemySFXAudioSource, enemyDeadClip);

                QuestManager.Instance.ProcessQuest(QuestType.DestroyEnemy, enemyID);

                GameManager.Instance.Main.playerStats.AddExp(exp);
            }
        }

        #endregion IDamageable
    }
}

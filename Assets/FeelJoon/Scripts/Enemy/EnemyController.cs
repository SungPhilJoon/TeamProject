using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        private ManualCollision enemyManualCollision;

        [Header("아이템 드롭")]
        [SerializeField] private ItemObjectDatabase database;

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

        public Animator EnemyAnimator => animator;

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
                Debug.Log(stateMachine.CurrentState);
            }

            stateMachine.Update(Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private void DropItem()
        {
            ItemObject dropItemObject = database.itemObjects[Random.Range(0, database.itemObjects.Length)];

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
                DropItem();
                stateMachine.ChangeState<EnemyDeadState>();
            }
        }

        #endregion IDamageable
    }
}

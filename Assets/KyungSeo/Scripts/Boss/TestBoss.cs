using System;
using System.Collections;
using System.Collections.Generic;
using UnityChanAdventure.FeelJoon;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace UnityChanAdventure.KyungSeo
{
    public enum BossState
    {
        None,
        Idle,
        Chase,
        Melee,
        Throw,
        Jump,
        Dead
    }

    public class TestBoss : MonoBehaviour, IAttackable, IDamageable
    {
        #region variables

        private StateMachine<EnemyController> stateMachine;
        private BossFieldOfView bossFOV;

        private EnemyType enemyType = EnemyType.Boss;

        [CanBeNull] public GameObject target;
        public GameObject weaponProjectile;
        public GameObject bossWeapon;
        public Vector3 startPosition;

        public int currentHp;
        public int maxHp = 100;
        public TMP_Text hpText;
        public int attackPower = 2;
        public int phaseNumber;
        private bool isCrazy = false;

        public float recognizeDistance = 20.0f;
        public float jumpDistance = 15.0f;
        public float throwDistance = 10.0f;
        public float attackDistance = 5.0f;

        public BossState bossState = BossState.None;
        private Animator animator;
        private NavMeshAgent agent;

        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack1 = Animator.StringToHash("Attack1");
        private static readonly int Attack2 = Animator.StringToHash("Attack2");
        private static readonly int MeleeAttack = Animator.StringToHash("MeleeAttack");
        private static readonly int ThrowAttack = Animator.StringToHash("ThrowAttack");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int SitDown = Animator.StringToHash("SitDown");
        private static readonly int GetAngry = Animator.StringToHash("GetAngry");

        #endregion

        #region Unity Methods

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            startPosition = transform.position;
            currentHp = maxHp;
            phaseNumber = 0;
        }

        private void Update()
        {
            CalcDistance();
            CheckHP();
        }

        private void OnDrawGizmos()
        {
            Vector3 gizmosCenter = transform.position;
            gizmosCenter.y = 0;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(gizmosCenter, recognizeDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gizmosCenter, jumpDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gizmosCenter, throwDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmosCenter, attackDistance);
        }

        #endregion

        #region Change State

        private void CalcDistance()
        {
            if (target == null)
            {
                bossState = BossState.None;
                return;
            }

            float distance = Vector3.Distance(target.transform.position, transform.position);
            // 무지성 if 드가자~ // if문 자체를 리뉴얼해야 하는데...
            if (distance <= attackDistance)
                ChangeState(BossState.Melee);
            else if (distance >= attackDistance && distance <= throwDistance)
                ChangeState(BossState.Throw);
            else if (distance >= throwDistance && distance <= recognizeDistance)
                ChangeState(BossState.Chase);
            else if (phaseNumber == 2 && distance >= jumpDistance)
                ChangeState(BossState.Jump);
            else
                stateMachine.ChangeState<EnemyIdleState>(); // 이렇게??
        }

        private void ChangeState(BossState newState)
        {
            if (bossState == newState) return;
            StopAllCoroutines();

            switch (newState)
            {
                case BossState.None:
                    bossState = BossState.None;
                    NoneState();
                    break;
                case BossState.Idle:
                    bossState = BossState.Idle;
                    IdleState();
                    break;
                case BossState.Melee:
                    bossState = BossState.Melee;
                    MeleeState();
                    break;
                case BossState.Throw:
                    bossState = BossState.Throw;
                    ThrowState();
                    break;
                case BossState.Jump:
                    bossState = BossState.Jump;
                    JumpState();
                    break;
                case BossState.Chase:
                    bossState = BossState.Chase;
                    ChaseState();
                    break;
            }
        }

        private IEnumerator WanderWhatToDoNext()
        {
            float waitTime = Random.Range(3.5f, 5.0f);
            yield return new WaitForSeconds(waitTime);

            int newState = Random.Range(2, 6);
            if (phaseNumber != 2 && newState == 5)
                newState = Random.Range(2, 5);
            ChangeState((BossState) newState);
        }

        private void CheckHP()
        {
            hpText.text = $"HP : {currentHp}";
            if (target != null && currentHp >= (maxHp >> 1))
                phaseNumber = 1;
            if (currentHp <= (maxHp >> 1))
            {
                phaseNumber = 2;
                OnNewPhase();
            }
        }

        private void NoneState()
        {
            Debug.Log("갔구나");
            agent.SetDestination(startPosition);
        }

        private void IdleState()
        {
            Debug.Log("너구나?");
            agent.ResetPath();
            animator.SetBool(Walk, false);
            transform.LookAt(target.transform);
        }

        private void ChaseState()
        {
            Debug.Log("쫓아간다...");
            animator.SetBool(Walk, true);
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }

        private void MeleeState()
        {
            Debug.Log("널 때릴거야");
            animator.SetBool(Walk, false);
            agent.ResetPath();
            OnMeleeAttack();
            StartCoroutine(WanderWhatToDoNext());
        }

        private void ThrowState()
        {
            Debug.Log("던진다");
            animator.SetBool(Walk, false);
            agent.ResetPath();
            OnThrowAttack();
            StartCoroutine(WanderWhatToDoNext());
        }

        private void JumpState()
        {
            Debug.Log("날아간다~!");
            animator.SetBool(Walk, false);
            agent.ResetPath();
            OnJumpAttack();
            StartCoroutine(WanderWhatToDoNext());
        }

        private void DeadState()
        {
            Debug.Log("죽었다...");
            animator.SetBool(Walk, false);
            agent.ResetPath();
            animator.SetTrigger(Die);
        }

        private void OnNewPhase()
        {
            if (isCrazy) return;
            isCrazy = true;
            Debug.Log("나 화났워");

            //광폭화
            animator.SetTrigger(GetAngry);
            attackPower = Mathf.RoundToInt(attackPower * 2.5f);
            agent.speed = 5;
            //Material bodyMaterial = GetComponentInChildren<MeshRenderer>().material;
            //bodyMaterial.color = Color.red;
        }

        #endregion

        #region User Setted Methods

        private void OnMeleeAttack()
        {
            Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(to - from);
            animator.SetTrigger(MeleeAttack);
        }

        private void OnThrowAttack()
        {
            Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(to - from);
            animator.SetTrigger(ThrowAttack);
        }
        
        public void OnThrow()
        {
            GameObject obj = Instantiate(weaponProjectile, bossWeapon.transform.position, bossWeapon.transform.rotation);
            obj.transform.parent = null;
        }

        private void OnJumpAttack()
        {
            Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(to - from);

            animator.SetTrigger(Jump);
            while (Vector3.Distance(to, from) <= 0.1f)
            {
                transform.Translate(to * Time.deltaTime);
            }

            animator.SetTrigger(SitDown);
        }

        #endregion

        #region Interface Methods

        public bool IsAlive { get; }

        public void TakeDamage(int damage, Transform target = null, GameObject hitEffectPrefab = null)
        {
            if (currentHp > 0)
                currentHp -= damage;

            if (currentHp <= 0)
            {
                currentHp = 0;
                bossState = BossState.Dead;
                DeadState();
            }
        }

        public AttackBehaviour CurrentAttackBehaviour { get; }
        
        public void OnExecuteMeleeAttack()
        {
            
        }

        public void OnExecuteProjectileAttack()
        {
            
        }
        
        #endregion
    }
}
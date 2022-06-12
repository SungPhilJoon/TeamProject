using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityChanAdventure.FeelJoon;
using Random = UnityEngine.Random;

public class BossJumpAndSit : State<BossController>
{
    #region Variables

    private Vector3 firstBossPosition;
    private Vector3 targetPosition;
    private float targetDistance;

    private float normalTime;
    private float jumpTime;

    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

    private bool isCheckCollision = false;

    protected readonly int hashJumpTrigger = Animator.StringToHash("JumpTrigger");
    protected readonly int hashAttackDistance = Animator.StringToHash("AttackDistance");

    #endregion Variables

    #region State

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
        agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        firstBossPosition = context.transform.position;
        targetPosition = context.Target.position;
        targetDistance = context.targetDistance;

        normalTime = 0f;
        jumpTime = targetDistance / 10f;

        isCheckCollision = false;

        context.transform.LookAt(context.Target);

        GameObject obj = GameObject.Instantiate(context.jumpAttackPlaceArea,
            targetPosition + (firstBossPosition - targetPosition).normalized * agent.stoppingDistance, 
            Quaternion.identity);

        GameObject.Destroy(obj, 2f);

        animator.SetTrigger(hashJumpTrigger);
        animator.SetFloat(hashAttackDistance, context.targetDistance);

        context.throwAttackCoolDown = 10;
        context.StartCoroutine(context.BossThrowAttackCoolTime());
    }

    public override void Update(float deltaTime)
    {
        if (controller.isGrounded)
        {
            agent.ResetPath();
            controller.Move(Vector3.zero);

            GameManager.Instance.CameraVibrateEffect(0.5f);

            if (!isCheckCollision)
            {
                isCheckCollision = true;
                CheckCollision();

                AudioManager.Instance.PlaySFX(
                AudioManager.Instance.enemySFXAudioSource,
                AudioManager.Instance.enemySFXClips,
                "BossAttackEffect");
            }
            
            if (stateMachine.ElapsedTimeInState > normalTime + 0.5f)
            {
                animator.SetTrigger(hashJumpTrigger);
                stateMachine.ChangeState<BossIdle>();
            }

            return;
        }

        if (context.transform.position.y >= firstBossPosition.y)
        {
            normalTime += deltaTime;
            Vector3 tempPos = Parabola(firstBossPosition, targetPosition, targetDistance, normalTime / jumpTime);

            agent.SetDestination(new Vector3(tempPos.x, firstBossPosition.y, tempPos.z));

            controller.Move(new Vector3(agent.velocity.x, tempPos.y, agent.velocity.z) * deltaTime);
        }
    }

    public override void OnExit()
    {
        AudioManager.Instance.PlaySFX(
        AudioManager.Instance.enemySFXAudioSource,
        AudioManager.Instance.enemySFXClips,
        "BossAttackEffect");
    }

    #endregion State

    #region Helper Methods

    private static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    private void CheckCollision()
    {
        Collider[] targetColliders = Physics.OverlapSphere(context.transform.position, 20.0f, context.targetMask);

        foreach (Collider targetCollider in targetColliders)
        {
            if (targetCollider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                int finalDamage = Mathf.RoundToInt(targetDistance) * 20;

                damageable.TakeDamage(finalDamage);
            }
        }
    }

    #endregion Helper Methods
}
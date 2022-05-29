using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ETeam.FeelJoon;

public class BossJumpAndSit : State<BossController>
{
    #region Variables

    private Vector3 firstBossPosition;
    private Vector3 targetPosition;
    private float targetDistance;

    private float normalTime;

    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

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

        context.transform.LookAt(context.Target);

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

            if (stateMachine.ElapsedTimeInState > normalTime + 1.5f)
            {
                Debug.Log("»£√‚");
                animator.SetTrigger(hashJumpTrigger);
                stateMachine.ChangeState<BossIdle>();
            }

            return;
        }

        if (context.transform.position.y >= firstBossPosition.y)
        {
            normalTime += deltaTime;
            Vector3 tempPos = Parabola(firstBossPosition, targetPosition, targetDistance, normalTime / 3);

            agent.SetDestination(new Vector3(tempPos.x, firstBossPosition.y, tempPos.z));

            controller.Move(new Vector3(agent.velocity.x, tempPos.y, agent.velocity.z) * deltaTime);
        }
        // context.transform.position = tempPos;
    }

    public override void OnExit()
    {
    }

    #endregion State

    #region Helper Methods

    private static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    #endregion Helper Methods
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

public class BossJumpAndSit : State<BossController>
{
    #region Variables

    private Vector3 firstBossPosition;
    private Vector3 targetPosition;
    private float targetDistance;

    private float normalTime;

    private Animator animator;

    protected readonly int hashJumpTrigger = Animator.StringToHash("JumpTrigger");
    protected readonly int hashAttackDistance = Animator.StringToHash("AttackDistance");

    #endregion Variables

    #region State

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
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
        //Vector3 jumpNormalizeDirection = (targetPosition - firstBossPosition).normalized;

        if (context.transform.position.y < 0f)
        {
            context.transform.position = new Vector3(context.transform.position.x, 0f, context.transform.position.z);
            stateMachine.ChangeState<BossIdle>();
            return;
        }

        if (context.transform.position.y >= targetDistance - 1)
        {
            animator.SetTrigger(hashJumpTrigger);
            Debug.Log("¾É¾Ò´Ï?");
        }

        normalTime += Time.deltaTime;
        Vector3 tempPos = Parabola(firstBossPosition, targetPosition, targetDistance, normalTime / 5);
        context.transform.position = tempPos;
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
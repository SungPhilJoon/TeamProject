using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

public class BossMeleeAttack : State<BossController>
{
    #region Variables
    private Animator animator;

    protected readonly int hashAttackTrigger = Animator.StringToHash("AttackTrigger");
    protected readonly int hashAttackDistance = Animator.StringToHash("AttackDistance");

    #endregion Variables

    #region State

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
    //    if (!context.isBossMeleeAttack1_Available)
    //    {
    //        stateMachine.ChangeState<BossIdle>();
    //        return;
    //    }

        context.transform.LookAt(context.Target);

        animator.SetTrigger(hashAttackTrigger);
        animator.SetFloat(hashAttackDistance, context.targetDistance);

        context.StartCoroutine(context.BossMeleeAttackCoolTime());
    }

    public override void Update(float deltaTime)
    {
        if(stateMachine.ElapsedTimeInState > context.coolTime)
        {
            stateMachine.ChangeState<BossIdle>();
        }
    }

    public override void OnExit()
    {
        
    }

    #endregion State
}

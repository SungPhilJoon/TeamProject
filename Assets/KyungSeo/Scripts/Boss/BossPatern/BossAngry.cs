using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

public class BossAngry : State<BossController>
{
    #region Variables

    private Animator animator;

    protected readonly int hashChangePhaseTrigger = Animator.StringToHash("ChangePhaseTrigger");

    #endregion Variables

    #region State

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.SetTrigger(hashChangePhaseTrigger);
        context.agent.speed *= 3f;
        context.increaseDamageAmount = 3;
    }

    public override void Update(float deltaTime)
    {
        if (stateMachine.ElapsedTimeInState > 3.0f)
        {
            stateMachine.ChangeState<BossIdle>();
        }
    }

    public override void OnExit()
    {

    }

    #endregion State
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityChanAdventure.FeelJoon;

public class BossRun : State<BossController>
{
    #region Variables

    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

    protected readonly int hashIsMove = Animator.StringToHash("isMove");

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
        animator.SetBool(hashIsMove, true);
        agent?.SetDestination(context.Target.position);
    }

    public override void Update(float deltaTime)
    {
        if (!context.IsAlive)
        {
            stateMachine.ChangeState<BossDead>();
            return;
        }

        Transform enemy = context.Target;
        if (enemy == null)
        {
            stateMachine.ChangeState<BossIdle>();
            return;
        }

        if (context.isBossThrowAttack_Available)
        {
            //animator.SetBool(hashIsMove, false);
            stateMachine.ChangeState<BossIdle>();
            return;
        }

        agent.SetDestination(context.Target.position);

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            controller.Move(agent.velocity * Time.deltaTime);
            return;
        }

        //animator.SetBool(hashIsMove, false);
        stateMachine.ChangeState<BossIdle>();
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }

    #endregion State
}

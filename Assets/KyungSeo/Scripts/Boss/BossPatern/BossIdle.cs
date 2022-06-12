using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;

public class BossIdle : State<BossController>
{
    #region Variables
    private Animator animator;

    protected readonly int hashIsMove = Animator.StringToHash("isMove");
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
        animator.SetBool(hashIsMove, false);
    }

    public override void Update(float deltaTime)
    {
        // 호옥시 모르니까 남겨놓겠음 최종빌드때 아무 문제 없으면 지워주세요
        // if (!context.IsAlive)
        // {
        //     stateMachine.ChangeState<BossDead>();
        //     return;
        // }

        Transform enemy = context.Target;
        
        if (enemy == null || !enemy.GetComponent<IDamageable>().IsAlive)
        {
            return;
        }

        if (context.isBossMeleeAttack_Available && context.IsAvailableMeleeAttack)
        {
            stateMachine.ChangeState<BossMeleeAttack>();
        }
        else if (context.IsAvailableThrowAttack && context.isBossThrowAttack_Available)
        {
            int randomSkillIndex = Random.Range(0, 2);
            switch (randomSkillIndex)
            {
                case 0:
                    stateMachine.ChangeState<BossThrowAttack>();
                    break;
                case 1:
                    if (context.bossPhase.Equals(1))
                    {
                        break;
                    }
                    stateMachine.ChangeState<BossJumpAndSit>();
                    break;
            }
        }
        else
        {
            stateMachine.ChangeState<BossRun>();
        }
    }

    public override void OnExit()
    {

    }

    #endregion State
}

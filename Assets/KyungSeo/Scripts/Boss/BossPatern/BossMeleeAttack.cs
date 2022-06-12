using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;
using Random = UnityEngine.Random;

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
        context.transform.LookAt(context.Target);

        GameObject obj = GameObject.Instantiate(context.attackPlaceArea, 
            context.BossManualCollision.transform.position - Vector3.up * 0.9f,
            context.BossManualCollision.transform.rotation);

        GameObject.Destroy(obj, 1.5f);

        animator.SetTrigger(hashAttackTrigger);
        animator.SetFloat(hashAttackDistance, context.targetDistance);

        context.StartCoroutine(context.BossMeleeAttackCoolTime());
    }

    public override void Update(float deltaTime)
    {
        if (stateMachine.ElapsedTimeInState > context.coolTime)
        {
            stateMachine.ChangeState<BossIdle>();
        }
    }

    public override void OnExit()
    {
        
    }

    #endregion State
}

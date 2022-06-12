using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;

public class BossAngry : State<BossController>
{
    #region Variables

    private Animator animator;
    private CharacterController controller;

    protected readonly int hashChangePhaseTrigger = Animator.StringToHash("ChangePhaseTrigger");

    #endregion Variables

    #region State

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
    }

    public override void OnEnter()
    {
        animator.SetTrigger(hashChangePhaseTrigger);
        controller.enabled = false;
        context.agent.speed *= 3f;
        context.increaseDamageAmount = 3;

        AudioManager.Instance.PlaySFX(
        AudioManager.Instance.enemySFXAudioSource,
        AudioManager.Instance.enemySFXClips,
        "BossTaunt");
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
        controller.enabled = true;
    }

    #endregion State
}

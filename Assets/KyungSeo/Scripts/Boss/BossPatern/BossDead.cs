using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;

public class BossDead : State<BossController>
{
    #region Variables
    private Animator animator;

    protected readonly int hashIsAlive = Animator.StringToHash("isAlive");
    protected readonly int hashOnDead = Animator.StringToHash("OnDead");

    #endregion Variables

    #region State
    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.SetBool(hashIsAlive, context.IsAlive);
        animator.SetTrigger(hashOnDead);

        AudioManager.Instance.PlayForceSFX(
        AudioManager.Instance.enemySFXAudioSource,
        AudioManager.Instance.enemySFXClips,
        "BossDead");
    }

    public override void Update(float deltaTime)
    {
        Time.timeScale = Mathf.Lerp(0.1f, 1f, stateMachine.ElapsedTimeInState / 1.5f);

        if (stateMachine.ElapsedTimeInState > 4.0f)
        {
            GameObject.Destroy(context.gameObject);
        }
    }

    public override void OnExit()
    {

    }

    #endregion State
}

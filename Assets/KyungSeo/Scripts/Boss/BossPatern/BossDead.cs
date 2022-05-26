using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.FeelJoon;

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
        Debug.Log("Á×¾ú´Ï?"); // ¿©·¯¹ø Á×³×...¤¾;
    }

    public override void Update(float deltaTime)
    {
        
    }

    public override void OnExit()
    {

    }

    #endregion State
}

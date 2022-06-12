using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.FeelJoon;

public class PlayerDash : State<PlayerController>
{
    #region Variables
    private Animator animator;

    private readonly int hashDashTrigger = Animator.StringToHash("DashTrigger");
    
    #endregion Variables

    public override void OnInitialized()
    {
        animator = context.GetComponentInChildren<Animator>();
    }

    public override void OnEnter()
    {
        animator.SetTrigger(hashDashTrigger);
    }

    public override void Update(float deltaTime)
    {
        
    }

    public override void OnExit()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class EnemyVictoryState : State<EnemyController>
    {
        #region Variables
        private Animator animator;

        protected readonly int hashIsPlayerDead = Animator.StringToHash("IsPlayerDead");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator.SetBool(hashIsPlayerDead, GameManager.Instance.IsPlayerDead);
        }

        public override void Update(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            
        }
    }
}
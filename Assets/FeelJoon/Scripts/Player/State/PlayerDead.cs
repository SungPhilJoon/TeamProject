using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class PlayerDead : State<PlayerController>
    {
        #region Variables
        private readonly int hashIsAlive = Animator.StringToHash("IsAlive");

        #endregion Variables

        public override void OnInitialized()
        {
            
        }

        public override void OnEnter()
        {
            context.animator.SetBool(hashIsAlive, context.IsAlive);
        }

        public override void Update(float deltaTime)
        {
            if (stateMachine.ElapsedTimeInState > 3.0f)
            {
                GameObject.Destroy(context.gameObject);
            }
        }

        public override void OnExit()
        {
            
        }
    }
}

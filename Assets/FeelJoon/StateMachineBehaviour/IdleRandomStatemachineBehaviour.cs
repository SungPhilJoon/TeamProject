using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRandomStatemachineBehaviour : StateMachineBehaviour
{
#region Variables
    public int numberOfStates = 2;
    public float minNormTime = 3.0f;
    public float maxNormTime = 5.0f;

    public float randomNormalTime;

    private readonly int hashIdleIndex = Animator.StringToHash("IdleIndex");

#endregion Variables

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        randomNormalTime = Random.Range(minNormTime, maxNormTime);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(hashIdleIndex, -1);
        }
        if(stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
        {
            animator.SetInteger(hashIdleIndex, Random.Range(1, numberOfStates));
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}

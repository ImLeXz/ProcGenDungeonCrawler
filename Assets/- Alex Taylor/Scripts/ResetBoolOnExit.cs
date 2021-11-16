using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResetBoolOnExit : StateMachineBehaviour
{
    [SerializeField]
    private string booleanVariableName;
    [SerializeField]
    private bool value;

    // OnStateExit is called when a transition ends and the state 
    //machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(booleanVariableName, value);
    }
}

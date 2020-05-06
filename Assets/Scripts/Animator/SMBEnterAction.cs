using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMBEnterAction : StateMachineBehaviour
{

    public string boolName;        
    public bool status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
            animator.SetBool("canMove", true);
            animator.ResetTrigger("combo");     //reseting combo trigger.
            animator.ResetTrigger("roll");      //resetng roll trigger.
    }

}

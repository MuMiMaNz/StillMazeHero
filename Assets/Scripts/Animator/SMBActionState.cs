using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMBActionState : StateMachineBehaviour
{

    //private string boolName = "canMove";
        
    //private bool status = true;

	public string boolName;
	public bool status;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		//Debug.Log("Attact anim exit , " + boolName +" : "+  status);
        animator.SetBool(boolName, status);
        
	}

    


}

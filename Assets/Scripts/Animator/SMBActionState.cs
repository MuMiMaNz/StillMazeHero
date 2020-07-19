using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMBActionState : StateMachineBehaviour
{

	public string boolName; // canMove
	public bool status; // true

	private PlayerController playerController;

	private void OnEnable() {
		playerController = GameObject.Find("PlayerController").GetComponent<PlayerController>();
	}

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
		playerController.normalAttack = true;
	}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
		
	}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		//Debug.Log("Attact anim exit , " + boolName +" : "+  status);
        animator.SetBool(boolName, status);
		playerController.normalAttack = false;
	}

}

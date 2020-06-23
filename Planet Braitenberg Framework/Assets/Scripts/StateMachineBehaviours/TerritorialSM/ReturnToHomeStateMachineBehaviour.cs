using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToHomeStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//set the attack torque to zero
		//territorialBehaviour.attackTorque = 0f;
		territorialBehaviour.motorTorque = 0f;
		//represent the fact that the vehicle is returning home
		animator.SetBool (TerritorialSMUtility.ParameterNames.ReturningToHome, true);
		//set the attacking mode of the vehicle to false
		territorialBehaviour.isAttacking = false;
	}


}

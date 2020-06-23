using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtHomeStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;
	internal Territory territory;


	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//set the attack torque parameter
		animator.SetFloat(TerritorialSMUtility.ParameterNames.AttackTorque, territorialBehaviour.attackTorque);
		//set the return to home torque parameter
		animator.SetFloat(TerritorialSMUtility.ParameterNames.ReturnToHomeTorque, territorialBehaviour.returnToHomeTorque);
		//set the rotate torque
		animator.SetFloat(TerritorialSMUtility.ParameterNames.RotateTorque, territorialBehaviour.rotateTorque);
		//set the rotation amoutn paramater
		animator.SetFloat(TerritorialSMUtility.ParameterNames.RotationAmount, territorialBehaviour.rotationAmount);
		//set the negative rotation amount parameter
		animator.SetFloat(TerritorialSMUtility.ParameterNames.NegativeRotationAmount, territorialBehaviour.rotationAmount * -1);
		if (this.territory == null)
			GameObject.FindGameObjectWithTag (TagManager.Territory);
		//set the returning to home parameter to false
		animator.SetBool(TerritorialSMUtility.ParameterNames.ReturningToHome, false);
	}
		
}



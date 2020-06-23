using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientingStateMachineBehaviour : StateMachineBehaviour {


	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//invert the vehicles motor torque
		territorialBehaviour.invertMotorTorque = false;
		float tempRotationAmount;
		tempRotationAmount = animator.GetFloat (TerritorialSMUtility.ParameterNames.RotationAmount) * animator.GetFloat (TerritorialSMUtility.ParameterNames.ObjectLocation);
		float tempNegativeRotationAmount;
		tempNegativeRotationAmount = animator.GetFloat (TerritorialSMUtility.ParameterNames.NegativeRotationAmount) * animator.GetFloat (TerritorialSMUtility.ParameterNames.ObjectLocation);
		animator.SetFloat (TerritorialSMUtility.ParameterNames.TempRotationAmount, tempRotationAmount);
		animator.SetFloat (TerritorialSMUtility.ParameterNames.TempNegativeRotationAmount, tempNegativeRotationAmount);
	}

}

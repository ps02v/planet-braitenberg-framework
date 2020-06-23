using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		territorialBehaviour.frontSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.TempRotationAmount);
		territorialBehaviour.rearSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.TempNegativeRotationAmount);
		territorialBehaviour.motorTorque = animator.GetFloat (TerritorialSMUtility.ParameterNames.RotateTorque);
	}
}

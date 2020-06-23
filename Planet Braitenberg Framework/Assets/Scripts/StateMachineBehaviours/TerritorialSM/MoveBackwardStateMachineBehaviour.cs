using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackwardStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;


	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		territorialBehaviour.invertMotorTorque = true;
		territorialBehaviour.frontSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.TempNegativeRotationAmount);
		territorialBehaviour.rearSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.TempRotationAmount);
		territorialBehaviour.motorTorque = animator.GetFloat (TerritorialSMUtility.ParameterNames.RotateTorque);
	}
}

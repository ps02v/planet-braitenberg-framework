using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversingStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//invert the vehicles motor torque
		territorialBehaviour.invertMotorTorque = true;
		//apply steer rotation
		territorialBehaviour.frontSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.RotationAmount);
		territorialBehaviour.rearSteerAngle = animator.GetFloat (TerritorialSMUtility.ParameterNames.NegativeRotationAmount);
		//apply motor torque
		territorialBehaviour.motorTorque = animator.GetFloat (TerritorialSMUtility.ParameterNames.RotateTorque);
	}


}

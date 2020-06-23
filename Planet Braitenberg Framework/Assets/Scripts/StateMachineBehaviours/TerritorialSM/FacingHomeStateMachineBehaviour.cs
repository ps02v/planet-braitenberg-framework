using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingHomeStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//invert the vehicles motor torque
		territorialBehaviour.invertMotorTorque = false;
				//apply steer rotation
		territorialBehaviour.frontSteerAngle = 0f;
		territorialBehaviour.rearSteerAngle = 0f;
		//apply motor torque
		territorialBehaviour.motorTorque = animator.GetFloat (TerritorialSMUtility.ParameterNames.ReturnToHomeTorque);
	}

}

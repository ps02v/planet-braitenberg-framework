using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingStateMachineBehaviour : StateMachineBehaviour {

	internal TerritorialBehaviourSM territorialBehaviour;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		territorialBehaviour.isAttacking = true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightMovementType {ForwardBack, SideToSide, LeftSideOnly, Circular};

public class LoomingLightController : MonoBehaviour {

	[Tooltip("A reference to the vehicle whose behaviour will be monitored.")]
	public Vehicle vehicle;
	[Tooltip("Specifies the movement of the looming light.")]
	public LightMovementType lightMovement = LightMovementType.ForwardBack; 

	private Animator anim;
	//private int activatedTriggerHash;


	// Use this for initialization
	void Start () {
		//get a reference to the animator component of the target light
		anim = GetComponent<Animator>();
		//this.activatedTriggerHash = Animator.StringToHash("VehicleActivated");

		if (vehicle == null) {
			//get a reference to the vehicle
			this.vehicle = MyRoutines.GetPrimaryVehicle();
		}
	}

	void UpdateLightMovement()
	{
		this.ResetLightBehaviour ();
		switch (this.lightMovement) {
		case LightMovementType.ForwardBack:
			anim.SetBool ("LoomingLight", true);
			break;
		case LightMovementType.SideToSide:
			anim.SetBool ("SideToSideLight", true);
			break;
		case LightMovementType.LeftSideOnly:
			anim.SetBool ("LeftSideOnlyLight", true);
			break;
		case LightMovementType.Circular:
			anim.SetBool ("CircularLight", true);
			break;
		}
	}

	void ResetLightBehaviour()
	{
		anim.SetBool ("LoomingLight", false);
		anim.SetBool ("SideToSideLight", false);
		anim.SetBool ("LeftSideOnlyLight", false);
		anim.SetBool ("CircularLight", false);
	}
	
	// Update is called once per frame
	void Update () {
		if (this.vehicle.disableMotor == false)
		{
			this.ResetLightBehaviour ();
			anim.SetBool ("VehicleActivated", true);
			return;
		}
		this.UpdateLightMovement ();
	}
}
